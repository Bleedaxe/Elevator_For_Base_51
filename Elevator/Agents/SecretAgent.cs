namespace Elevator_For_Base_51.Agents
{
	using System.Collections.Generic;

	public class SecretAgent : Agent
	{
		private List<FloorType> accessibleFloors;

		public SecretAgent(Elevator elevator, string name)
			: base(elevator, name)
		{
			this.accessibleFloors = new List<FloorType>(new FloorType[] { FloorType.G, FloorType.S });
		}

		public override IReadOnlyList<FloorType> AccessibleFloors => accessibleFloors;
	}
}
