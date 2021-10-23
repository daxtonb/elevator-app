using System;
using ElevatorApp.Core;

namespace ElevatorApp.Test
{
    public class MockFactory
    {
        private static readonly double _defaultOccupantWeight = 150;
        private static readonly double _defaultElevatorMaxWeight = 1000;
        private static readonly int _defaultFloorCount = 10;
        private static readonly int _defaultElevatorCount = 2;

        public Occupant CreateOccupant(Building building = null)
        {
            if (building == null)
            {
                building = new Building(_defaultFloorCount, _defaultElevatorCount, _defaultElevatorMaxWeight);
            }

            foreach (var elevator in building.Elevators)
            {
                elevator.StateChanged += handleStateChanged;
                elevator.DirectionChanged += handleDirectionChanged;
                elevator.FloorChanged += handleFloorChanged;
            }

            return new Occupant(building, _defaultOccupantWeight);
        }

        public Building CreateBuilding() => new Building(_defaultFloorCount, _defaultElevatorCount, _defaultElevatorMaxWeight);
        public Elevator CreateElevator(Building building)
        {
            var elevator = new Elevator(building, _defaultElevatorMaxWeight);
            elevator.StateChanged += handleStateChanged;
            elevator.DirectionChanged += handleDirectionChanged;
            elevator.FloorChanged += handleFloorChanged;
            return elevator;
        }

        private void handleStateChanged(Elevator sender, Elevator.StateChangedEventArgs eventArgs)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay}\tState changed | {sender}");
        }

        private void handleDirectionChanged(Elevator sender, Elevator.DirectionChangedEventArgs eventArgs)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay}\tDirection changed | {sender}");
        }

        private void handleFloorChanged(Elevator sender, Elevator.FloorChangedEventArgs eventArgs)
        {
            Console.WriteLine($"{DateTime.Now.TimeOfDay}\tFloor changed | {sender}");
        }

    }
}