namespace Elevator_For_Base_51
{
	using Agents;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	public class Elevator
	{
		private readonly Semaphore semaphore;
		private readonly ManualResetEvent isDoorOpenEvent;
		private readonly ManualResetEvent someoneCanLeave;

		private FloorType currentFloor;
		private readonly int capacity;
		private readonly HashSet<IAgent> agents;

		private readonly object lockMovement = new object();
		private readonly object lockAgents = new object();

		public Elevator(int capacity, FloorType startFloor = FloorType.G)
		{
			semaphore = new Semaphore(capacity, capacity);
			isDoorOpenEvent = new ManualResetEvent(false);
			someoneCanLeave = new ManualResetEvent(true);

			this.capacity = capacity;
			currentFloor = startFloor;
			agents = new HashSet<IAgent>();

		}

		public void Call(FloorType floor)
		{
			while (true)
			{
				if (IsDoorOpen() && floor == currentFloor)
				{
					return;
				}

				lock (lockMovement)
				{
					if (floor != currentFloor)
					{
						Console.WriteLine("ELEVETOR IS CALLED ON FLOOR " + floor);
						MoveToFloor(floor);
					}

					if (CanDoorBeOpened())
					{
						someoneCanLeave.Set();
						return;
					}
				}
				Thread.Sleep(200);
			}			
		}

		public bool TryEnter(IAgent agent, FloorType floor)
		{
			if (!isDoorOpenEvent.WaitOne(0) && currentFloor == floor)
			{
				if (semaphore.WaitOne(0))
				{
					this.agents.Add(agent);
					return true;
				}				
			}
			return false;
		}

		private void Leave(IAgent agent)
		{
			lock (lockAgents)
			{
				this.agents.Remove(agent);
				semaphore.Release();
			}
		}

		public bool IsDoorOpen()
		{
			return !isDoorOpenEvent.WaitOne(0);
		}


		public void GoTo(FloorType floor, IAgent agent)
		{
			using (var cts = new CancellationTokenSource())
			{
				Task task = new Task(() => this.Elevate(floor, agent, cts.Token), cts.Token);
				task.Start();
				while (someoneCanLeave.WaitOne(0) && agent.DesiredFloor != currentFloor) { }
				cts.Cancel();
			}
			Leave(agent);
		}


		private void Elevate(FloorType floor, IAgent agent, CancellationToken ct)
		{
			lock (lockMovement)
			{
				ct.ThrowIfCancellationRequested();
				Console.WriteLine("Movement is LOCKED!!!!!");
				Console.WriteLine($"{agent.ToString()} PRESSED BUTTON FOR FLOOR {floor}");
				CloseDoor();
				while (true)
				{
					isDoorOpenEvent.WaitOne();
					MoveToFloor(floor);
					if (CanDoorBeOpened())
					{
						isDoorOpenEvent.Reset();
						someoneCanLeave.Set();
						return;
					}
					if (agent.CanEnterFloor(floor))
					{
						return;
					}
					agent.ChangeFloor();
					floor = agent.DesiredFloor;
				}
			}
		}

		private void MoveToFloor(FloorType floor)
		{
			while (currentFloor != floor)
			{
				Thread.Sleep(1000);
				currentFloor = currentFloor < floor
					? currentFloor + 1
					: currentFloor - 1;
			}
			Console.WriteLine($"{DateTime.Now}: ELEVATOR IS ON FLOOR {floor}");
		}
		private bool CanDoorBeOpened()
		{
			return this.agents
				.All(a => a.CanEnterFloor(currentFloor)) || agents.Count == 0;
		}

		private void CloseDoor()
		{
			Thread.Sleep(3000);
			isDoorOpenEvent.Set();
			Console.WriteLine($"{DateTime.Now}: Elevator door is closing at floor " + currentFloor);
			someoneCanLeave.Reset();
		}
	}
}
