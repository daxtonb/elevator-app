using System;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Represents an elevator for a building
    /// </summary>
    public class Elevator
    {
        /// <summary>
        /// Maximum weight allowed in elevator
        /// </summary>
        private readonly double maxWeight;

        /// <summary>
        /// Maximum weight allowed in elevator
        /// </summary>
        /// <param name="maxWeight"></param>
        public Elevator(double maxWeight)
        {
            if (maxWeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxWeight), maxWeight, $"{maxWeight} is not a valid weight. Value must be greater than 0");
            }

            this.maxWeight = maxWeight;
        }
    }
}