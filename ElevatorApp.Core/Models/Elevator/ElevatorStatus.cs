using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Deals with elevator status
    /// </summary>
    public partial class Elevator
    {

        /// <summary>
        /// Lock for thread-safe access to the list of current requested floors
        /// </summary>
        private readonly object _requestsLock = new object();

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

        private double _currentHeight = 0;
        private DateTime? _doorsOpenedDateTime;

        /// <summary>
        /// The current floor that the elevator is located. Starts on ground level.
        /// </summary>
        public int CurrentFloor => (int)Math.Floor(_currentHeight / _building.FloorHeight) + 1;

        public bool IsDirectionUp => _currentDirection == Direction.Up;
        public bool IsDirectionDown => _currentDirection == Direction.Down;
        public bool IsNoDirection => _currentDirection == Direction.None;
        public bool IsMoving => _currentState == State.Moving;
        public bool IsDoorsOpen => _currentState == State.DoorsOpen;
        public bool IsDoorsClosed => _currentState == State.DoorsClosed;
        public bool IsReady => _currentState == State.Ready;
        public bool IsAtDestinationFloor => _currentRequest != null && _currentRequest.FloorNumber == CurrentFloor;

        /// <summary>
        /// Adds floor number the list of requested floors
        /// </summary>
        /// <param name="floorNumber"></param>
        private Task AddRequestAsync(Request request)
        {
            return Task.Run(() =>
            {
                lock (_requestsLock)
                {
                    if (!_requests.Contains(request))
                    {
                        _requests.Add(request);
                    }
                }

                return SetNextRequestAsync();
            });
        }

        /// <summary>
        /// Removes floor number from the list of requested floors
        /// </summary>
        /// <param name="floorNumber"></param>
        private Task RemoveRequestAsync(Request request)
        {
            return Task.Run(() =>
            {
                lock (_requestsLock)
                {
                    _requests.Remove(request);
                }
            });
        }

        /// <summary>
        /// Safely returns request list
        /// </summary>
        /// <returns></returns>
        private Task<List<Request>> GetRequestsAsync()
        {
            return Task.Run(() =>
            {
                lock (_requestsLock)
                {
                    return _requests;
                }
            });
        }

        /// <summary>
        /// Evaluates current requests and decides the next request to execute
        /// </summary>
        private Task SetNextRequestAsync()
        {
            return Task.Run(() =>
            {
                Request nextRequestUp, nextRequestDown;

                lock (_requestsLock)
                {
                    nextRequestUp = _requests.Where(r => r.FloorNumber > CurrentFloor).OrderBy(r => r.FloorNumber).FirstOrDefault();
                    nextRequestDown = _requests.Where(r => r.FloorNumber < CurrentFloor).OrderByDescending(r => r.FloorNumber).FirstOrDefault();
                }

                if (IsDirectionUp)
                {
                    if (nextRequestUp != null)
                        SetCurrentRequest(nextRequestUp);
                    else if (nextRequestDown != null)
                        SetCurrentRequest(nextRequestDown);
                    else
                        SetCurrentRequest(null);
                }
                else if (IsDirectionDown)
                {
                    if (nextRequestDown != null)
                        SetCurrentRequest(nextRequestDown);
                    else if (nextRequestUp != null)
                        SetCurrentRequest(nextRequestUp);
                    else
                        SetCurrentRequest(null);
                }
                else
                {
                    if (nextRequestUp != null && nextRequestDown != null)
                    {
                        int differenceUp = nextRequestUp.FloorNumber - CurrentFloor;
                        int differenceDown = CurrentFloor - nextRequestDown.FloorNumber;

                        if (differenceUp < differenceDown)
                            SetCurrentRequest(nextRequestUp);
                        else
                            SetCurrentRequest(nextRequestDown);
                    }
                    else
                    {
                        SetCurrentRequest(nextRequestUp ?? nextRequestDown);
                    }
                }

            });
        }

        private async void SetCurrentRequest(Request request)
        {
            if (request == null)
            {
                await SetCurrentDirectionAsync(Direction.None);
            }
            else if (request.FloorNumber > CurrentFloor)
            {
                await SetCurrentDirectionAsync(Direction.Up);
            }
            else if (request.FloorNumber < CurrentFloor)
            {
                await SetCurrentDirectionAsync(Direction.Down);
            }
            else
            {
                await SetCurrentDirectionAsync(Direction.None);
            }

            _currentRequest = request;
        }

        /// <summary>
        /// Returns true if any ruests are available
        /// </summary>
        /// <returns></returns>
        private Task<bool> IsAnyRequestsAsync()
        {
            return Task.Run(() =>
            {
                lock (_requestsLock)
                {
                    return _requests.Any();
                }
            });
        }

        /// <summary>
        /// Adds occupant the list of occupants
        /// </summary>
        /// <param name="occupant"></param>
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

        /// <summary>
        /// Sets the current state of the elevator
        /// </summary>
        public Task SetCurrentStateAsync(State state)
        {
            return Task.Run(() =>
            {
                lock (_currentStateLock)
                {
                    _currentState = state;
                    OnStateChanged(new StateChangedEventArgs(state));
                }
            });
        }

        /// <summary>
        /// Gets the current state of the elevator
        /// </summary>
        public State GetCurrentState() => _currentState;

        /// <summary>
        /// Sets the current Direction of the elevator
        /// </summary>
        public Task SetCurrentDirectionAsync(Direction direction)
        {
            return Task.Run(() =>
            {
                lock (_currentDirectionLock)
                {
                    _currentDirection = direction;
                    OnDirectionChanged(new DirectionChangedEventArgs(direction));
                }
            });
        }

        /// <summary>
        /// Gets the current Direction of the elevator
        /// </summary>
        public Direction GetCurrentDirection() => _currentDirection;
    }
}