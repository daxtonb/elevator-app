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

        /// <param name="floorCount">Total number of floors for building</param>
        /// <param name="elevatorCount">Total number of elevators for building</param>
        public Building(int floorCount, int elevatorCount)
        {
            if (floorCount <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(floorCount), floorCount, $"{floorCount} is not a valid floor count. Value must be greater than 1.");
            }

            if (elevatorCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(elevatorCount), elevatorCount, $"{elevatorCount} is not a valid elevator count. Value must be at least 1");
            }

            FloorCount = floorCount;
            Elevators = new Elevator[elevatorCount];
            occupants = new List<Occupant>();
        }
    }
}