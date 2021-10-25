using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElevatorApp.Core.Utils;
using Microsoft.Extensions.Logging;

namespace ElevatorApp.Core
{
    /// <summary>
    /// This partial class handles all things concerning the elevator's status
    /// </summary>
    public partial class Elevator
    {
        /// <summary>
        /// Lock for thread-safe acces to the elevator's current state
        /// </summary>
        private readonly object _currentStateLock = new object();

        /// <summary>
        /// Lock for thread-safe acces to the elevator's current direction
        /// </summary>
        private readonly object _currentDirectionLock = new object();

        /// <summary>
        /// Lock for thread-safe access of occupants
        /// </summary>
        private readonly object _occupantsLock = new object();

        /// <summary>
        /// Lock for thread-safe access of requests
        /// </summary>
        private readonly object _disembarkRequestsLock = new object();

        /// <summary>
        /// Lock for thread-safe access of requests
        /// </summary>
        private readonly object _boardRequestsLock = new object();

        /// <summary>
        /// Current state of the elevator
        /// </summary>
        private State _currentState;

        /// <summary>
        /// Current direction of the elevator
        /// </summary>
        /// 
        private Direction _currentDirection;

        /// <summary>
        /// Represents the floor number of the destination floor
        /// </summary>
        private Request _currentRequest;

        /// <summary>
        /// Current height of the elevator, measured in feet above ground level
        /// </summary>
        private double _currentHeight = 0;

        /// <summary>
        /// Date-time stamp that the doors were last opened
        /// </summary>
        private DateTime? _doorsOpenedDateTime;

        /// <summary>
        /// The current floor that the elevator is located. Starts on ground level.
        /// </summary>
        public int CurrentFloor => (int)Math.Floor(_currentHeight / _building.FloorHeight) + 1;
        public int? RequestedFloor => _currentRequest?.FloorNumber;
        public bool IsDirectionUp => CurrentDirection == Direction.Up;
        public bool IsDirectionDown => CurrentDirection == Direction.Down;
        public bool IsNoDirection => CurrentDirection == Direction.None;
        public bool IsMoving => CurrentState == State.Moving;
        public bool IsDoorsOpen => CurrentState == State.DoorsOpen;
        public bool IsDoorsClosed => CurrentState == State.DoorsClosed;
        public bool IsReady => CurrentState == State.Ready;
        public bool IsAtDestinationFloor => _currentRequest != null && _currentRequest.FloorNumber == CurrentFloor && _currentHeight % _building.FloorHeight == 0;
        public int OccupantsCount => _occupants.Count;
        public double Capcity => Math.Round(((double)_currentWeight / _maxWeight) * 100);

        /// <summary>
        /// Current state of elevator
        /// </summary>
        public State CurrentState 
        {
            private set 
            {
                if (_currentState != value)
                {
                    StateChanged?.Invoke(this, new StateChangedEventArgs(_currentState, value));

                    lock (_currentStateLock)
                    {
                        _currentState = value;
                    }
                }
            }

            get
            {
                lock (_currentStateLock)
                {
                    return _currentState;
                }
            }
        }

        /// <summary>
        /// Current direction of elevator
        /// </summary>
        public Direction CurrentDirection
        {
            private set 
            {
                if (_currentDirection != value)
                {
                    lock (_currentDirectionLock)
                    {
                        _currentDirection = value;
                    }
                    DirectionChanged?.Invoke(this, new DirectionChangedEventArgs(value));
                }
            }

            get 
            {
                lock (_currentDirectionLock)
                {
                    return _currentDirection;
                }
            }
        }

        /// <summary>
        /// Evaluates current requests and decides the next request to execute
        /// </summary>
        private Task SetNextRequestAsync()
        {
            return Task.Run(() =>
            {
                var nextBoardRequest = GetNextBoardRequest();
                var nextDisembarkRequest = GetNextDisembarkRequest();

                // CONDITION: Elevator is not yet at full capacity. Assign next closest request. Otherwise,
                // only service disembark requests.
                if (Capcity < 100)
                {
                    _currentRequest = RequestHelper.GetClosestRequest(new Request[] { nextBoardRequest, nextDisembarkRequest }, this);
                }
                else
                {
                    _currentRequest = nextDisembarkRequest;
                }

                SetCurrentDirection();
            });
        }

        /// <summary>
        /// Sets the current direction the elevator
        /// </summary>
        /// <param name="request">Request to set</param>
        private void SetCurrentDirection()
        {
            if (_currentRequest == null)
            {
                CurrentDirection = Direction.None;
            }
            else if (_currentRequest.FloorNumber > CurrentFloor)
            {
                CurrentDirection = Direction.Up;
            }
            else if (_currentRequest.FloorNumber < CurrentFloor)
            {
                CurrentDirection = Direction.Down;
            }
            else if (_currentRequest is BoardRequest boardRequest)
            {
                CurrentDirection = boardRequest.Direction;
            }
            else
            {
                CurrentDirection = Direction.None;
            }
        }
        
        /// <summary>
        /// Adds occupant to the list of occupants
        /// </summary>
        /// <param name="occupant">Occupant to add to elevator</param>
        private Task AddOccupantAsync(Occupant occupant)
        {
            return Task.Run(() =>
            {
                lock (_occupantsLock)
                {
                    if (!_occupants.Contains(occupant))
                    {
                        _occupants.Add(occupant);
                    }
                }

                RemoveRequest<BoardRequest>(CurrentFloor);
            });
        }

        /// <summary>
        /// Removes occupant from the list of occupants
        /// </summary>
        /// <param name="occupant"></param>
        private Task RemoveOccupantAsync(Occupant occupant)
        {
            return Task.Run(() =>
            {
                lock (_occupantsLock)
                {
                    _occupants.Remove(occupant);
                }

                RemoveRequest<DisembarkRequest>(CurrentFloor);

                // Unflagged board requests now that room has been freed up
                lock (_boardRequestsLock)
                {
                    foreach (var request in _boardRequests)
                    {
                        request.IsFlaggedForLater = false;
                    }
                }
            });
        }
    }
}