using System.Collections.Generic;

namespace Elevator_For_Base_51.Agents
{
	public class ConfidentialAgent : Agent
	{
		private List<FloorType> accessibleFloors;

		public ConfidentialAgent(Elevator elevator, string name) 
			:base(elevator, name)
		{
			this.accessibleFloors = new List<FloorType>(new FloorType[] { FloorType.G });
		}

		public override IReadOnlyList<FloorType> AccessibleFloors => accessibleFloors;
	}
}
