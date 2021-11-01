namespace ElevatorApp.Core
{
    public static class ElevatorConstants
    {
        /// <summary>
        /// Time in seconds to close elevator doors
        /// </summary>
        public static readonly int TIME_TO_CLOSE_DOORS = 1; // 1 s

        /// <summary>
        /// Time in milliseconds to poll for elevator updates
        /// </summary>
        public static readonly int ELAPSE_TIME = 500;  // 500 ms (0.5 s)

        /// <summary>
        /// Max speed of elevator, measured in feet per second
        /// </summary>
        public static readonly int MAX_SPEED = 3;   // 3 ft/s

        /// <summary>
        /// Height in feet of each floor in building
        /// </summary>
        public static readonly double FLOOR_HEIGHT = 9; // 9 ft

    }
}