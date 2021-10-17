using System;
using System.Collections.Generic;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Represents a building containing any number of elevators
    /// </summary>
    public class Building
    {
        /// <summary>
        /// Total number of floors for building
        /// </summary>
        public int FloorCount { get; }

        /// <summary>
        /// All elvators for building
        /// </summary>
        public Elevator[] Elevators { get; }

        /// <summary>
        /// All occupants for building
        /// </summary>
        public List<Occupant> occupants { get; }

        /// <summary>
        /// Height of each floor in the building, measured in feet.
        /// </summary>
        public double FloorHeight { get; } = 10;

        /// <param name="floorCount">Total number of floors for building</param>
        /// <param name="elevatorCount">Total number of elevators for building</param>
        public Building(int floorCount, int elevatorCount, double maxElevatorWeight)
        {
            if (floorCount <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(floorCount), floorCount, $"{floorCount} is not a valid floor count. Value must be greater than 1.");
            }

            if (elevatorCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(elevatorCount), elevatorCount, $"{elevatorCount} is not a valid elevator count. Value must be at least 1");
            }

            if (elevatorCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(elevatorCount), elevatorCount, $"{elevatorCount} is not a valid elevator count. Value must be at least 1");
            }

            FloorCount = floorCount;
            occupants = new List<Occupant>();


            Elevators = new Elevator[elevatorCount];

            for (int i = 0; i < elevatorCount; i++)
            {
                Elevators[i] = new Elevator(this, maxElevatorWeight);
            }
        }
    }
}