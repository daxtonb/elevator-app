using System;
using System.Threading.Tasks;
using ElevatorApp.Core;
using Microsoft.AspNetCore.SignalR;

namespace ElevatorApp.Server.Hubs
{
    /// <summary>
    /// SignlarR hub that handles updates from Elevator events
    /// </summary>
    public class ElevatorHub : Hub
    {
        private readonly Building _building;

        public ElevatorHub(Building building)
        {
            _building = building;
        }

        public Occupant CreateOccupant()
        {
            return new Occupant(_building, 150);
        }
    }
}