namespace ElevatorApp.Core
{
    /// <summary>
    /// Models a request for an elevator from a floor. A user requests to take an elevator up or down.
    /// </summary>
    public class BoardRequest : Request
    {
        /// <summary>
        /// Requested direction to take elevator
        /// </summary>
        public Elevator.Direction Direction { get; }

        /// <summary>
        /// Flags a request for later (i.e., elevator is full)
        /// </summary>
        public bool IsFlaggedForLater { get; set; } = false;

        /// <param name="occupant">Occupant making request</param>
        /// <param name="direction">Occupant's desired direction of travel</param>
        public BoardRequest(Occupant occupant, Elevator.Direction direction) : base(occupant.CurrentFloor)
        {
            Direction = direction;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}