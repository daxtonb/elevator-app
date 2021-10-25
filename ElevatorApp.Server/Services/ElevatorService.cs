using System;
using System.Linq;
using ElevatorApp.Core;
using ElevatorApp.Server.Hubs;
using ElevatorApp.Server.Models;
using Microsoft.AspNetCore.SignalR;

namespace ElevatorApp.Server.Services
{
    public class ElevatorService
    {
        private readonly Building _building;
        private readonly IHubContext<ElevatorHub> _hub;

        public ElevatorService(ServerBuilding building, IHubContext<ElevatorHub> hub)
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
            foreach (var item in ElevatorHub.OccupantsByConnectionId)
            {
                if (item.Value.Id == occupant.Id)
                {
                    await _hub.Clients.Client(item.Key).SendAsync("OccupantUpdated", OccupantViewModel.From(occupant));
                    break;
                }
            }
        }
    }
}