using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorApp.Core;
using ElevatorApp.Server.Models;
using ElevatorApp.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace ElevatorApp.Server.Hubs
{
    /// <summary>
    /// SignlarR hub that handles updates from Elevator events
    /// </summary>
    public class ElevatorHub : Hub
    {
        /// <summary>
        /// Building represented on server
        /// </summary>
        private readonly Building _building;

        /// <summary>
        /// Elevator service for sending notifications and data to client
        /// </summary>
        private readonly ElevatorService _elevatorService;

        /// <summary>
        /// Current occupants in session
        /// </summary>
        /// <typeparam name="string">Occupant's connection ID</typeparam>
        /// <typeparam name="Occupant">Occupant</typeparam>
        public static Dictionary<string, Occupant> OccupantsByConnectionId { get; } = new Dictionary<string, Occupant>();


        /// <param name="building">Server's Building instance</param>
        /// <param name="elevatorService">Elevator service</param>
        public ElevatorHub(ServerBuilding building, ElevatorService elevatorService)
        {
            _building = building;
            _elevatorService = elevatorService;
        }

        /// <summary>
        /// Creates an occupant for each client connection and registers event handlers
        /// </summary>
        public override Task OnConnectedAsync()
        {
            var occupant = new Occupant(_building, 150);
            OccupantsByConnectionId.Add(Context.ConnectionId, occupant);
            _building.Occupants.Add(occupant);

            occupant.StateChanged += _elevatorService.SendOccupantUpdate;
            occupant.RequestedFloorChanged += _elevatorService.SendOccupantUpdate;
            occupant.CurrentFloorChanged += _elevatorService.SendOccupantUpdate;

            return base.OnConnectedAsync();
        }

        /// <summary>
        /// Client disconnect handler to remove occupants from being tracked.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            OccupantsByConnectionId.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Client request for an elevator
        /// </summary>
        /// <param name="direction">Direction to travel</param>
        public Task RequestElevatorAsync(Elevator.Direction direction)
        {
            var occupant = OccupantsByConnectionId[Context.ConnectionId];
            return occupant.RequestElevatorAsync(direction);
        }

        /// <summary>
        /// Client request to be picked up by elevator
        /// </summary>
        /// <param name="floorNumber">Desired floor number</param>
        public Task RequestFloorAsync(int floorNumber)
        {
            var occupant = GetClientOccupant();
            return occupant.RequestFloorAsync(floorNumber);
        }

        /// <summary>
        /// Client request to retrieve all elevators
        /// </summary>
        public IEnumerable<ElevatorViewModel> RequestElevators()
        {
            return _building.Elevators.Select(e => ElevatorViewModel.From(e));
        }

        /// <summary>
        /// Client request for occupant representing client
        /// </summary>
        /// <returns></returns>
        public OccupantViewModel RequestOccupant()
        {
            return OccupantViewModel.From(GetClientOccupant());
        }

        /// <summary>
        /// Client request for bulding
        /// </summary>
        public BuildingViewModel RequestBuilding()
        {
            return BuildingViewModel.From(_building);
        }

        /// <summary>
        /// Returns the occupant for current connection
        /// </summary>
        private Occupant GetClientOccupant()
        {
            if (OccupantsByConnectionId.TryGetValue(Context.ConnectionId, out Occupant occupant))
            {
                return occupant;
            }

            throw new Exception("Occupant does not exist");
        }
    }
}