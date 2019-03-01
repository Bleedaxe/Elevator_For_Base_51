namespace Elevator_For_Base_51.Agents
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public abstract class Agent : IAgent
	{
		private FloorType currentFloor = FloorType.G;
		private readonly string name;

		private ManualResetEvent hasEnteredElevator = new ManualResetEvent(false);
		private ManualResetEvent hasLeftElevator = new ManualResetEvent(false);

		private readonly object lockObj = new object();

		private readonly Elevator elevator;

		private Random random;

		protected Agent(Elevator elevator, string name)
		{
			this.name = name;
			this.elevator = elevator;
			random = new Random();
		}

		public abstract IReadOnlyList<FloorType> AccessibleFloors { get; }

		public FloorType DesiredFloor { get; private set; }

		public void DoWork()
		{
			while (true)
			{
				var actionIndex = random.Next(4);
				switch (actionIndex)
				{
					case 0:
						Console.WriteLine($"{DateTime.Now}: {name} is working at floor {currentFloor}");
						Thread.Sleep(3000);
						break;
					case 1:
						Console.WriteLine($"{DateTime.Now}: {name} is taking a break for 5 minutes at floor {currentFloor}");
						Thread.Sleep(5000);
						break;
					case 2:
						this.hasEnteredElevator.Reset();
						this.hasLeftElevator.Reset();

						Task task = new Task(() => this.CallElevator());
						task.Start();

						Console.WriteLine($"{DateTime.Now}: {{{this.GetType().Name}}} {name} is waiting for elevator on floor {currentFloor}.");
						hasEnteredElevator.WaitOne();
						Console.WriteLine($"{DateTime.Now}: {{{this.GetType().Name}}} {name} has entered elevator on floor {currentFloor}.");
						hasLeftElevator.WaitOne();
						Console.WriteLine($"{DateTime.Now}: {{{this.GetType().Name}}} {name} is on floor {currentFloor}.");
						task.Wait();
						break;
					case 3:
						Console.WriteLine($"{DateTime.Now}: {name} has finished for today. Goodbye :)");
						return;
				}
			}
		}

		public bool CanEnterFloor(FloorType floor)
		{
			return this.AccessibleFloors
				.Any(f => f == floor);
		}

		public void PickFloor()
		{
			var floorsCount = Enum.GetValues(typeof(FloorType)).Length;
			FloorType randomFloor;
			do
			{
				randomFloor = (FloorType)random.Next(floorsCount);
			}
			while (randomFloor == DesiredFloor);
			this.DesiredFloor = randomFloor;
		}

		public void ChangeFloor()
		{
			var previousFloor = this.DesiredFloor;
			PickFloor();
			Console.WriteLine($"{DateTime.Now}: {{{this.GetType().Name}}} {name} can't enter floor {previousFloor}. He will try to enter floor {this.DesiredFloor}");
		}

		private void EnterElevator()
		{
			if (!elevator.TryEnter(this, currentFloor))
			{
				CallElevator();
			}
			hasEnteredElevator.Set();
			this.GoTo();
		}

		private void GoTo()
		{
			PickFloor();
			Console.WriteLine($"{DateTime.Now}: {{{this.GetType().Name}}} {name} wants to go to floor {DesiredFloor}");
			elevator.GoTo(this.DesiredFloor, this);
			this.LeaveElevator();
		}

		private void CallElevator()
		{
			elevator.Call(currentFloor);
			EnterElevator();
		}

		private void LeaveElevator()
		{
			this.currentFloor = this.DesiredFloor;
			this.hasLeftElevator.Set();
		}

		public override string ToString()
		{
			return name;
		}
	}
}
