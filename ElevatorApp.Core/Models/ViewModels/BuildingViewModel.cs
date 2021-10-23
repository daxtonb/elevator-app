namespace ElevatorApp.Core
{
    public class BuildingViewModel
    {
        public int FloorCount { get; set; }

        public static BuildingViewModel From(Building building)
        {
            return new BuildingViewModel
            {
                FloorCount = building.FloorCount
            };
        }
    }
}