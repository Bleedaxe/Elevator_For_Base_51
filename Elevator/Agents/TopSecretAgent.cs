namespace Elevator_For_Base_51.Agents
{
	using System.Collections.Generic;

	public class TopSecretAgent : Agent
	{
		private List<FloorType> accessibleFloors;

		public TopSecretAgent(Elevator elevator, string name)
			: base(elevator, name)
		{
			this.accessibleFloors = new List<FloorType>(new FloorType[] { FloorType.G, FloorType.S, FloorType.T1, FloorType.T2 });
		}

		public override IReadOnlyList<FloorType> AccessibleFloors => accessibleFloors;
	}
}
