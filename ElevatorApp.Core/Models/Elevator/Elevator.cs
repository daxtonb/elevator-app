using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Contains basic elevator fields, properties, and methods
    /// </summary>
    public partial class Elevator
    {
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
        private double _currentWeight = 0;

        /// <summary>
        /// Maximum speed of the elevator, measured in feet per second
        /// </summary>
        private static readonly double _maxSpeed = 2;

        /// <summary>
        /// Elevator requests 
        /// </summary>
        private readonly List<Request> _requests;

        private readonly List<Occupant> _occupants;

        /// <summary>
        /// Time for the elevator to wait until closing doors, measured in seonds
        /// </summary>
        private static readonly int _timeToCloseDoors = 10;

        /// <summary>
        /// Time in milliseconds to schedule checks and fire events
        /// </summary>
        private static readonly int _elapseTime = 1000; // Check every second

        /// <summary>
        /// Timer for timed events
        /// </summary>
        private readonly System.Timers.Timer _timer;

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

            _timer.Interval = _elapseTime;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Submit's a user's request to take the elevator to a specified floor
        /// </summary>
        /// <param name="floorNumber">Desired floor's number</param>
        /// <returns>Task fulfilling the request</returns>
        public Task RequestFloorAsync(int floorNumber)
        {
            if (floorNumber < 1 || floorNumber > _building.FloorCount)
            {
                throw new ArgumentOutOfRangeException(nameof(floorNumber), floorNumber, $"{floorNumber} is not a valid floor.");
            }

            var request = new Request(floorNumber);

            lock (_requestsLock)
            {
                if (!_requests.Any(r => r.Equals(request)))
                {
                    AddRequestAsync(request).Wait();
                }
            }

            return SetNextRequestAsync();
        }

        /// <summary>
        /// Returns true if an occupant can enter from his/her current floor
        /// </summary>
        /// <param name="occupant">Occupant desiring to enter elevator</param>
        public bool CanEnter(Occupant occupant)
        {
            return _currentState != State.Moving                        // Elevator must not be moving
                    && occupant.CurrentFloor == occupant.CurrentFloor   // Occupant must be on the same floor as the stopped elevator
                    && _currentWeight + occupant.Weight < _maxWeight;   // Occupant must not bring elevator over its weight capacity
        }

        public bool CanExit(Occupant occupant)
        {
            return _currentState != State.Moving;
        }

        /// <summary>
        /// Enter occupant into elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        public async void EnterAsync(Occupant occupant)
        {
            if (!CanEnter(occupant))
            {
                throw new Exception("Occupant may not enter");
            }

            if (_currentState == State.DoorsClosed)
            {
                await OpenDoorsAsync();
            }

            await AddOccupantAsync(occupant);
            _currentWeight += occupant.Weight;
        }

        /// <summary>
        /// Remove occupant from elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        public async void ExitAsync(Occupant occupant)
        {
            if (!CanExit(occupant))
            {
                throw new Exception("Occupant may not exit");
            }

            if (_currentState == State.DoorsClosed)
            {
                await OpenDoorsAsync();
            }

            await RemoveOccupantAsync(occupant);
            _currentWeight -= occupant.Weight;
        }

        /// <summary>
        /// Open elevator doors for occupants to leave or enter
        /// </summary>
        private Task OpenDoorsAsync()
        {
            if (_currentState == State.Moving)
            {
                throw new Exception("Doors may not be opened while elevator is moving.");
            }

            if (_currentState == State.DoorsClosed)
            {
                return SetCurrentStateAsync(State.DoorsOpen);
            }

            _doorsOpenedDateTime = DateTime.UtcNow;

            return null;
        }

        /// <summary>
        /// Close elevator doors
        /// </summary>
        private Task CloseDoorsAsync()
        {
            SetNextRequestAsync().Wait();
            return SetCurrentStateAsync(State.DoorsClosed);
        }

        #region Class Enums

        public enum Direction { None, Up, Down }
        public enum State { Ready, DoorsOpen, DoorsClosed, Moving }

        #endregion
    }
}