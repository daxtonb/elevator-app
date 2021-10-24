using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElevatorApp.Core.Utils;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Deals with elevator status
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

        public bool IsDirectionUp => CurrentDirection == Direction.Up;
        public bool IsDirectionDown => CurrentDirection == Direction.Down;
        public bool IsNoDirection => CurrentDirection == Direction.None;
        public bool IsMoving => CurrentState == State.Moving;
        public bool IsDoorsOpen => CurrentState == State.DoorsOpen;
        public bool IsDoorsClosed => CurrentState == State.DoorsClosed;
        public bool IsReady => CurrentState == State.Ready;
        public bool IsAtDestinationFloor => _currentRequest != null && _currentRequest.FloorNumber == CurrentFloor;
        public int OccupantsCount => _occupants.Count;
        public double Capcity => Math.Round(_currentWeight / _maxWeight);

        public State CurrentState 
        {
            private set 
            {
                if (_currentState != value)
                {

                    lock (_currentStateLock)
                    {
                        _currentState = value;
                    }
                    StateChanged?.Invoke(this, new StateChangedEventArgs(value));
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
                var closestRequest = RequestHelper.GetClosestRequest(new Request[] { nextBoardRequest, nextDisembarkRequest }, this);

                SetCurrentRequest(closestRequest);
            });
        }

        /// <summary>
        /// Sets the current request for the elevator
        /// </summary>
        /// <param name="request">Request to set</param>
        private void SetCurrentRequest(Request request)
        {
            if (request == null)
            {
                CurrentDirection = Direction.None;
            }
            else if (request.FloorNumber > CurrentFloor)
            {
                CurrentDirection = Direction.Up;
            }
            else if (request.FloorNumber < CurrentFloor)
            {
                CurrentDirection = Direction.Down;
            }
            else if (request is BoardRequest boardRequest)
            {
                CurrentDirection = boardRequest.Direction;
            }
            else
            {
                CurrentDirection = Direction.None;
            }

            _currentRequest = request;
        }

        private Task RemoveRequestAsync(int floorNumber)
        {
            return Task.Run(() =>
            {
                lock (_boardRequestsLock)
                {
                    for (int i = 0; i < _boardRequests.Count; i++)
                    {
                        if (_boardRequests[i].FloorNumber == floorNumber)
                        {
                            _boardRequests.RemoveAt(i);
                            break;
                        }
                    }

                }

                lock (_disembarkRequestsLock)
                {
                    for (int i = 0; i < _disembarkRequests.Count; i++)
                    {
                        if (_disembarkRequests[i].FloorNumber == floorNumber)
                        {
                            _disembarkRequests.RemoveAt(i);
                            break;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds occupant to the list of occupants
        /// </summary>
        /// <param name="occupant">Occupant to add to elevator</param>
        private Task AddOccupantAsync(Occupant occupant)
        {
            return Task.Run(async () =>
            {
                lock (_occupantsLock)
                {
                    if (!_occupants.Contains(occupant))
                    {
                        _occupants.Add(occupant);
                    }
                }

                await RemoveRequestAsync(CurrentFloor);
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
            });
        }
    }
}