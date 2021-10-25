using ElevatorApp.Core;

namespace ElevatorApp.Test
{
    public class MockOccupant : Occupant
    {
        public Building Building => _building;
        public MockOccupant(Building building, double weight, int startFloor = 1) : base(building, weight)
        {
            _currentFloor = startFloor;
        }
    }
}