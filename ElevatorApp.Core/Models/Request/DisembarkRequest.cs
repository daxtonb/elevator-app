namespace ElevatorApp.Core
{
    /// <summary>
    /// Models a request from inside the elevator to a specified floor.
    /// </summary>
    public class DisembarkRequest : Request
    {
        /// <param name="floorNumber">Number of request floor</param>
        public DisembarkRequest(int floorNumber) : base(floorNumber) { }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}