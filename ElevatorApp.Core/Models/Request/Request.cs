namespace ElevatorApp.Core
{
    /// <summary>
    /// Models a request for the elevator to go to a specifed floor
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Destination floor
        /// </summary>
        public int FloorNumber { get; }

        /// <param name="floorNumber">Destination floor number</param>
        public Request(int floorNumber)
        {
            FloorNumber = floorNumber;
        }

        public override bool Equals(object obj)
        {
            if (obj is BoardRequest request)
            {
                return request.FloorNumber == FloorNumber;
            }

            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}