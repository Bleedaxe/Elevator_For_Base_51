using Elevator_For_Base_51.Agents;
using System;
using System.Threading.Tasks;

namespace Elevator_For_Base_51
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Enter elevator capacity: ");
			int elevatorCapacity;
			while (!int.TryParse(Console.ReadLine(), out elevatorCapacity) || elevatorCapacity <= 0)
			{
				Console.Write("Enter elevator capacity: ");
			}

			Elevator elevator = new Elevator(elevatorCapacity);

			Console.Write("Enter agents count: ");
			int agentsCount;
			while (!int.TryParse(Console.ReadLine(), out agentsCount) || agentsCount <= 0)
			{
				Console.Write("Enter agents count: ");
			}

			var tasks = new Task[agentsCount];
			for (int i = 0; i < agentsCount; i++)
			{
				var agent = CreateAgent(elevator, $"Agent-{i + 1}");
				tasks[i] = new Task(() => agent.DoWork());
				tasks[i].Start();
			}

			foreach (var task in tasks) task.Wait();

			Console.Write("End of the day. Press any key to finish the program.");
			Console.ReadKey(false);
		}

		private static IAgent CreateAgent(Elevator elevator, string name)
		{
			int randomAgent = new Random().Next(3);
			switch (randomAgent)
			{
				case 0: return new ConfidentialAgent(elevator, name);
				case 1:	return new SecretAgent(elevator, name);
				case 2: return new TopSecretAgent(elevator, name);
				default: throw new IndexOutOfRangeException("Agent index out of range!");
			}
		}
	}
}
