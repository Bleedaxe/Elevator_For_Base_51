namespace Elevator_For_Base_51.Agents
{
	using System.Threading;

	public interface IAgent
	{
		FloorType DesiredFloor { get; }

		bool CanEnterFloor(FloorType floor);
		void ChangeFloor();
		void DoWork();
	}
}
