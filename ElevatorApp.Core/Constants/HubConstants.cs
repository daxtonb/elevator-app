namespace ElevatorApp.Core
{
    public static class HubConstants
    {
        public static readonly string URL_PATH = "/ElevatorHub";
        public static readonly string RECEIEVE_ELEVATOR_UPDATE = "ReceiveElevatorUpdate";
        public static readonly string CREATE_OCCUPANT = "CreateOccupant";
        public static readonly string RECEIVE_OCCUPANT = "ReceiveOccupant";
        public static readonly string RECEIVE_BUILDING = "ReceiveBuilding";
    }
}