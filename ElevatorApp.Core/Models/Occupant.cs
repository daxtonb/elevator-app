namespace ElevatorApp.Core
{
    /// <summary>
    /// Represents a building occupant that can ride in elevator
    /// </summary>
    public class Occupant
    {
        /// <summary>
        /// Weight in lbs of occupant
        /// </summary>
        public double Weight { get; }
        /// <summary>
        /// Floor that occupant is currently on. Default value is 1 (ground floor)
        /// </summary>
        public int CurrentFloor { get; set; } = 1;
        /// <summary>
        /// Floor that occupant wishes to ride elevator to
        /// </summary>
        public int RequestedFloor { get; set; }

        /// <param name="weight">Weight of occupant</param>
        public Occupant(double weight)
        {
            Weight = weight;
        }
    }
}