namespace ElevatorApp.Core
{
    public class ElevatorViewModel
    {
        public int Id { get; set; }
        public Elevator.State State { get; set; }
        public int OccupantCount { get; set; }
        public double Capacity { get; set; }
        public int CurrentFloor { get; set; }
        public Elevator.Direction CurrentDirection { get; set; }

        public static ElevatorViewModel From(Elevator elevator)
        {
            return new ElevatorViewModel()
            {
                Id = elevator.Id,
                State = elevator.CurrentState,
                OccupantCount = elevator.OccupantsCount,
                Capacity = elevator.Capcity,
                CurrentFloor = elevator.CurrentFloor,
                CurrentDirection = elevator.GetCurrentDirection()
            };
        }
    }
}