namespace ElevatorApp.Core
{
    public class OccupantViewModel
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public int? RequestedFloor { get; set; }
        public Occupant.State CurrentState { get; set; }
        public int? ElevatorId { get; set; }

        public static OccupantViewModel From(Occupant occupant)
        {
            return new OccupantViewModel()
            {
                Id = occupant.Id,
                CurrentFloor = occupant.CurrentFloor,
                RequestedFloor = occupant.RequestedFloor,
                CurrentState = occupant.CurrentState,
                ElevatorId = occupant.Elevator?.Id
            };
        }
    }
}