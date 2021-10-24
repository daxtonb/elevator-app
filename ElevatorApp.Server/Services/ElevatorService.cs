using System;
using System.Linq;
using ElevatorApp.Core;
using ElevatorApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ElevatorApp.Server.Services
{
    public class ElevatorService
    {
        private readonly Building _building;
        private readonly IHubContext<ElevatorHub> _hub;

        public ElevatorService(Building building, IHubContext<ElevatorHub> hub)
        {
            _building = building;
            _hub = hub;
            SetEventHandlers();
        }

        private void SetEventHandlers()
        {
            foreach (var elevator in _building.Elevators)
            {
                elevator.StateChanged += SendElevatorUpdate;
                elevator.FloorChanged += SendElevatorUpdate;
                elevator.DirectionChanged += SendElevatorUpdate;
            }
        }
        private async void SendElevatorUpdate(Elevator elevator, EventArgs eventArgs)
        {
            await _hub.Clients.All.SendAsync("ReceiveElevatorUpdate", ElevatorViewModel.From(elevator));
        }

        public async void SendOccupantUpdate(Occupant occupant, EventArgs eventArgs)
        {
            var client = ElevatorHub.OccupantsByConnectionId.First(o => o.Value.Id == occupant.Id);
            await _hub.Clients.Client(client.Key).SendAsync("OccupantUpdated", OccupantViewModel.From(occupant));
        }
    }
}