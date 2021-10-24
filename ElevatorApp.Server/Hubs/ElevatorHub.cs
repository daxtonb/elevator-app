using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorApp.Core;
using ElevatorApp.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace ElevatorApp.Server.Hubs
{
    /// <summary>
    /// SignlarR hub that handles updates from Elevator events
    /// </summary>
    public class ElevatorHub : Hub
    {
        private readonly Building _building;
        private readonly ElevatorService _elevatorService;
        private static Dictionary<string, Occupant> _occupantsByConnectionId = new Dictionary<string, Occupant>();

        public ElevatorHub(Building building, ElevatorService elevatorService)
        {
            _building = building;
            _elevatorService = elevatorService;
        }

        public override Task OnConnectedAsync()
        {
            base.OnConnectedAsync();

            var occupant = new Occupant(_building, 150);
            _occupantsByConnectionId.Add(Context.ConnectionId, occupant);
            _building.Occupants.Add(occupant);

            Clients.Caller.SendAsync(HubConstants.RECEIVE_OCCUPANT, OccupantViewModel.From(occupant)).Wait();

            return Clients.Caller.SendAsync(HubConstants.RECEIVE_BUILDING, BuildingViewModel.From(_building));
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _occupantsByConnectionId.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public Task RequestElevatorAsync(Elevator.Direction direction)
        {
            var occupant = _occupantsByConnectionId[Context.ConnectionId];
            return _building.RequestAsync(occupant, direction);
        }

        public IEnumerable<ElevatorViewModel> RequestElevators()
        {
            return _building.Elevators.Select(e => ElevatorViewModel.From(e));
        }

        public OccupantViewModel RequestOccupant()
        {
            if (_occupantsByConnectionId.TryGetValue(Context.ConnectionId, out Occupant occupant))
            {
                return OccupantViewModel.From(occupant);
            }

            throw new Exception("Occupant does not exist");
        }

        public BuildingViewModel RequestBuilding()
        {
            return BuildingViewModel.From(_building);
        }
    }
}