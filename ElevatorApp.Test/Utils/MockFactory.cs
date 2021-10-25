using System;
using ElevatorApp.Core;

namespace ElevatorApp.Test
{
    public class MockFactory
    {
        private static readonly double _defaultOccupantWeight = 150;
        private static readonly double _defaultElevatorMaxWeight = 300;
        private static readonly int _defaultFloorCount = 10;
        private static readonly int _defaultElevatorCount = 2;

        public Occupant CreateOccupant(Building building = null, int startFloor = 1)
        {
            if (building == null)
            {
                building = new MockBuilding(_defaultFloorCount, _defaultElevatorCount, _defaultElevatorMaxWeight);
            }

            return new MockOccupant(building, _defaultOccupantWeight, startFloor);
        }

        public Building CreateBuilding() => new MockBuilding(_defaultFloorCount, _defaultElevatorCount, _defaultElevatorMaxWeight);
        public Elevator CreateElevator(Building building)
        {
            var elevator = new Elevator(building, _defaultElevatorMaxWeight);
            return elevator;
        }
    }
}