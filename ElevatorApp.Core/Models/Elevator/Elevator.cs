using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorApp.Core.Utils;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Contains basic elevator fields, properties, and methods
    /// </summary>
    public partial class Elevator
    {
        /// <summary>
        /// Number of elevators in memory
        /// </summary>
        public static int elevatorCount = 0;

        /// <summary>
        /// Elevator's unique identifier
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Building in which elevator belongs to
        /// </summary>
        private readonly Building _building;

        /// <summary>
        /// Maximum weight allowed in elevator
        /// </summary>
        private readonly double _maxWeight;

        /// <summary>
        /// Current weight of elevator
        /// </summary>
        private double _currentWeight => _occupants.Sum(o => o.Weight);

        /// <summary>
        /// Maximum speed of the elevator, measured in feet per second
        /// </summary>
        private static readonly double _maxSpeed = ElevatorConstants.MAX_SPEED;

        /// <summary>
        /// Occupants inside of elevator
        /// </summary>
        private readonly List<Occupant> _occupants;

        /// <summary>
        /// Requests from occupants inside of the elvator
        /// </summary>
        private readonly List<DisembarkRequest> _disembarkRequests;

        /// <summary>
        /// Requests from occupants outside of the elevator
        /// </summary>
        private readonly List<BoardRequest> _boardRequests;

        /// <summary>
        /// Time for the elevator to wait until closing doors, measured in seonds
        /// </summary>
        private static readonly int _timeToCloseDoors = ElevatorConstants.TIME_TO_CLOSE_DOORS;

        /// <summary>
        /// Time in milliseconds to schedule checks and fire events
        /// </summary>
        private static readonly int _elapseTime = ElevatorConstants.ELAPSE_TIME;

        /// <summary>
        /// Timer for timed events
        /// </summary>
        private readonly System.Timers.Timer _timer;

        public Elevator() { }

        /// <param name="building">Building to which elevator belongs to</param>
        /// <param name="maxWeight">Maximum weight allowed in elevator</param>
        public Elevator(Building building, double maxWeight)
        {
            if (maxWeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxWeight), maxWeight, $"{maxWeight} is not a valid weight. Value must be greater than 0");
            }

            _building = building;
            _maxWeight = maxWeight;
            _timer = new System.Timers.Timer();
            _disembarkRequests = new List<DisembarkRequest>();
            _boardRequests = new List<BoardRequest>();
            _occupants = new List<Occupant>();

            _timer.Interval = _elapseTime;
            _timer.Enabled = true;
            _timer.Elapsed += OnTimeElapsed;

            Id = ++elevatorCount;
        }

        /// <summary>
        /// Returns true if an occupant can enter from his/her current floor
        /// </summary>
        /// <param name="occupant">Occupant desiring to enter elevator</param>
        public bool CanEnter(Occupant occupant)
        {
            return CurrentState != State.Moving                        // Elevator must not be moving
                    && occupant.CurrentFloor == occupant.CurrentFloor   // Occupant must be on the same floor as the stopped elevator
                    && _currentWeight + occupant.Weight <= _maxWeight;  // Occupant must not bring elevator over its weight capacity
        }

        /// <summary>
        /// Returns true if an occupant can exit the elevator
        /// </summary>
        /// <param name="occupant"></param>
        /// <returns>Occupant desiring to exit elevator</returns>
        public bool CanExit(Occupant occupant)
        {
            return CurrentState != State.Moving;
        }

        /// <summary>
        /// Enter occupant into elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        public Task EnterAsync(Occupant occupant)
        {
            if (CanEnter(occupant))
            {
                if (_currentState == State.DoorsClosed)
                {
                    OpenDoors();
                }

                return AddOccupantAsync(occupant);
            }
            else if (_currentRequest is BoardRequest boardRequest)
            {
                boardRequest.IsFlaggedForLater = true;
            }

            throw new Exception("Occupant cannot enter elevator");
        }

        /// <summary>
        /// Remove occupant from elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        public async void ExitAsync(Occupant occupant)
        {
            if (CanExit(occupant))
            {

                if (CurrentState == State.DoorsClosed)
                {
                    OpenDoors();
                }

                await RemoveOccupantAsync(occupant);
            }
        }

        /// <summary>
        /// Add request to store
        /// </summary>
        /// <param name="request">Request to be added</param>
        /// <typeparam name="T">Request</typeparam>
        public Task AddRequestAsync<T>(T request) where T : Request
        {
            if (request is BoardRequest boardRequest)
            {
                lock (_boardRequestsLock)
                {
                    _boardRequests.Add(boardRequest);
                }
            }
            else if (request is DisembarkRequest disembarkRequest)
            {
                lock (_disembarkRequestsLock)
                {
                    _disembarkRequests.Add(disembarkRequest);
                }
            }

            OnRequestMade(new RequestMadeEventArgs(request));
            return SetNextRequestAsync();
        }

        /// <summary>
        /// Remove request from store
        /// </summary>
        /// <param name="floorNumber">Request to be removed</param>
        /// <typeparam name="T">Request</typeparam>
        public void RemoveRequest<T>(int floorNumber) where T : Request
        {
            if (typeof(T) == typeof(BoardRequest))
            {
                lock (_boardRequestsLock)
                {
                    _boardRequests.RemoveAll(r => r.FloorNumber == floorNumber);
                }
            }
            else if (typeof(T) == typeof(DisembarkRequest))
            {
                lock (_disembarkRequestsLock)
                {
                    _disembarkRequests.RemoveAll(r => r.FloorNumber == floorNumber);
                }
            }
        }

        /// <summary>
        /// Open elevator doors for occupants to leave or enter
        /// </summary>
        private void OpenDoors()
        {
            if (_currentState == State.Moving)
            {
                throw new Exception("Doors may not be opened while elevator is moving.");
            }

            if (_currentState == State.DoorsClosed || _currentState == State.Ready)
            {
                _doorsOpenedDateTime = DateTime.UtcNow;
                CurrentState = State.DoorsOpen;
            }
        }

        /// <summary>
        /// Returns the next closest disembark request that is consistent with the elevator's current direction
        /// </summary>
        private DisembarkRequest GetNextDisembarkRequest()
        {
            DisembarkRequest nextUp, nextAlongTheWay, nextDown;

            lock (_disembarkRequestsLock)
            {
                var requestsOrderedAscending = _disembarkRequests.OrderBy(r => r.FloorNumber);
                var requestsOrderedDescending = _disembarkRequests.OrderByDescending(r => r.FloorNumber);

                if (IsDirectionUp)
                {
                    if (IsMoving)
                        nextAlongTheWay = requestsOrderedAscending.FirstOrDefault(r => r.FloorNumber > CurrentFloor + 1);
                    else
                        nextAlongTheWay = requestsOrderedAscending.FirstOrDefault(r => r.FloorNumber >= CurrentFloor);
                        
                    nextDown = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber < CurrentFloor);

                    return nextAlongTheWay ?? nextDown;
                }
                else if (IsDirectionDown)
                {
                    if (IsMoving)
                        nextAlongTheWay = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber < CurrentFloor - 1);
                    else
                        nextAlongTheWay = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber <= CurrentFloor);

                    nextUp = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber > CurrentFloor);

                    return nextAlongTheWay ?? nextUp;
                }
                else
                {
                    return RequestHelper.GetClosestRequest(_disembarkRequests, this) as DisembarkRequest;
                }
            }
        }

        /// <summary>
        /// Returns the next closest board request that is consistent with the elevator's current direction
        /// </summary>
        private BoardRequest GetNextBoardRequest()
        {
            BoardRequest nextUp, nextAlongTheWay, nextDown;

            lock (_disembarkRequestsLock)
            {
                var requestsOrderedAscending = _boardRequests.Where(r => !r.IsFlaggedForLater).OrderBy(r => r.FloorNumber);
                var requestsOrderedDescending = _boardRequests.Where(r => !r.IsFlaggedForLater).OrderByDescending(r => r.FloorNumber);

                if (IsDirectionUp)
                {
                    if (IsMoving)
                    {
                        nextUp = requestsOrderedAscending.FirstOrDefault(r => r.Direction == Elevator.Direction.Up && r.FloorNumber > CurrentFloor);
                        nextAlongTheWay = requestsOrderedAscending.FirstOrDefault(r => r.FloorNumber > CurrentFloor);
                    }
                    else
                    {
                        nextUp = requestsOrderedAscending.FirstOrDefault(r => r.Direction == Elevator.Direction.Up && r.FloorNumber >= CurrentFloor);
                        nextAlongTheWay = requestsOrderedAscending.FirstOrDefault(r => r.FloorNumber >= CurrentFloor);
                    }
                    nextDown = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber < CurrentFloor);

                    return nextUp ?? nextAlongTheWay ?? nextDown;
                }
                else if (IsDirectionDown)
                {
                    if (IsMoving)
                    {
                        nextDown = requestsOrderedDescending.FirstOrDefault(r => r.Direction == Elevator.Direction.Down && r.FloorNumber < CurrentFloor);
                        nextAlongTheWay = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber < CurrentFloor);
                    }
                    else
                    {
                        nextDown = requestsOrderedDescending.FirstOrDefault(r => r.Direction == Elevator.Direction.Down && r.FloorNumber <= CurrentFloor);
                        nextAlongTheWay = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber <= CurrentFloor);
                    }
                    nextUp = requestsOrderedDescending.FirstOrDefault(r => r.FloorNumber > CurrentFloor);

                    return nextDown ?? nextAlongTheWay ?? nextUp;
                }
                else
                {
                    return RequestHelper.GetClosestRequest(_boardRequests.Where(r => !r.IsFlaggedForLater), this) as BoardRequest;
                }
            }
        }

        /// <summary>
        /// Close elevator doors
        /// </summary>
        private Task CloseDoorsAsync()
        {
            CurrentState = State.DoorsClosed;
            _doorsOpenedDateTime = null;
            return SetNextRequestAsync();
        }

        /// <summary>
        /// Get all active disembark requests
        /// </summary>
        public IEnumerable<DisembarkRequest> GetDisembarkRequests() => _disembarkRequests;

        public override string ToString()
        {
            return $"Elevator {Id}, state: {CurrentState}, direction: {_currentDirection}, floor: {CurrentFloor}, occupants: {_occupants.Count}, capacity: {_currentWeight / _maxWeight}";
        }

        #region Class Enums

        public enum Direction { None, Up, Down }
        public enum State { Ready, DoorsOpen, DoorsClosed, Moving }

        #endregion
    }
}