using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, Occupant> _occupantsByConnectionId;

        public ElevatorHub(Building building)
        {
            _building = building;
            _occupantsByConnectionId = new Dictionary<string, Occupant>();
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
    }
}