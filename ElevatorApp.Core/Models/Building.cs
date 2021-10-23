using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorApp.Core.Utils;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Represents a building containing any number of elevators
    /// </summary>
    public class Building
    {

        /// <summary>
        /// Total number of floors for building
        /// </summary>
        public int FloorCount { get; }

        /// <summary>
        /// All elvators for building
        /// </summary>
        public Elevator[] Elevators { get; }

        /// <summary>
        /// All occupants for building
        /// </summary>
        public List<Occupant> Occupants { get; }

        /// <summary>
        /// Height of each floor in the building, measured in feet.
        /// </summary>
        public double FloorHeight { get; } = 10;

        /// <param name="floorCount">Total number of floors for building</param>
        /// <param name="elevatorCount">Total number of elevators for building</param>
        public Building(int floorCount, int elevatorCount, double maxElevatorWeight)
        {
            if (floorCount <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(floorCount), floorCount, $"{floorCount} is not a valid floor count. Value must be greater than 1.");
            }

            if (elevatorCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(elevatorCount), elevatorCount, $"{elevatorCount} is not a valid elevator count. Value must be at least 1");
            }

            if (elevatorCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(elevatorCount), elevatorCount, $"{elevatorCount} is not a valid elevator count. Value must be at least 1");
            }

            FloorCount = floorCount;
            Occupants = new List<Occupant>();


            Elevators = new Elevator[elevatorCount];

            for (int i = 0; i < elevatorCount; i++)
            {
                Elevators[i] = new Elevator(this, maxElevatorWeight);
                Elevators[i].StateChanged += ElevatorStateChangedHandler;
            }
        }

        public void ElevatorStateChangedHandler(Elevator elevator, Elevator.StateChangedEventArgs eventArgs)
        {
            if (eventArgs.State == Elevator.State.DoorsOpen)
            {
                foreach (var occupant in Occupants)
                {
                    if (occupant.CurrentState == Occupant.State.waiting && occupant.CurrentFloor == elevator.CurrentFloor)
                    {
                        occupant.NotifyElevatorReadyAsync(elevator).Wait();
                    }
                }
            }
        }

        /// <summary>
        /// Dispatches request to the next available elevator
        /// </summary>
        /// <param name="request">Occupant's request</param>
        private Task DispatchRequest(BoardRequest request)
        {
            return Task.Run(() =>
            {
                var elevator = ChooseElevator(request);
                elevator.AddBoardRequestAsync(request);
            });
        }

        /// <summary>
        /// Determines elevator best suited to fulfill request.
        /// </summary>
        /// <param name="request">Board request</param>
        private Elevator ChooseElevator(BoardRequest request)
        {
            var elevator = GetElevatorAlongTheWay(request);

            if (elevator == null)
            {
                elevator = GetNearestElevator(Elevators, request);
            }

            return elevator;
        }

        /// <summary>
        /// Queries elevators for elevator that can fulfill the request along its current path
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Elevator GetElevatorAlongTheWay(BoardRequest request)
        {
            IEnumerable<Elevator> elevators;
            if (request.Direction == Elevator.Direction.Up)
            {
                elevators = Elevators.Where(e => e.GetCurrentDirection() == request.Direction && e.CurrentFloor < request.FloorNumber);
            }
            else
            {
                elevators = Elevators.Where(e => e.GetCurrentDirection() == request.Direction && e.CurrentFloor > request.FloorNumber);
            }

            return GetNearestElevator(elevators, request);
        }

        /// </summary>
        /// <param name="elevators"></param>
        /// <param name="request"></param>
        /// <returns></returns>//
        private Elevator GetNearestElevator(IEnumerable<Elevator> elevators, BoardRequest request)
        {
            Elevator closest = null;

            foreach (var current in elevators)
            {
                if (current == null)
                {
                    continue;
                }
                if (closest == null)
                {
                    closest = current;
                }
                else
                {
                    if (Math.Abs(current.CurrentFloor - request.FloorNumber) < Math.Abs(closest.CurrentFloor - request.FloorNumber))
                    {
                        closest = current;
                    }
                }
            }

            return closest;
        }

        public Task RequestAsync(Occupant occupant, Elevator.Direction direction)
        {
            return DispatchRequest(new BoardRequest(occupant, direction));
        }
    }
}