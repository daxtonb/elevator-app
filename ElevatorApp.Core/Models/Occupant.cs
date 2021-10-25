using System;
using System.Threading.Tasks;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Represents a building occupant that can ride in elevator
    /// </summary>
    public class Occupant
    {
        private static int _occupantCount = 0;

        /// <summary>
        /// Unique identifeier for Occupant
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Building in which occupant belongs to
        /// </summary>
        protected readonly Building _building;

        /// <summary>
        /// Requested direction for elevator
        /// </summary>
        private Elevator.Direction _requestedDirection;

        /// <summary>
        /// Floor number of requested floor
        /// </summary>
        private int? _requestedFloor;

        /// <summary>
        /// Current state of occupant
        /// </summary>
        private State _currentState;

        /// <summary>
        /// Elevator of occupant
        /// </summary>
        private Elevator _elevator;

        /// <summary>
        /// Floor that occupant is currently on. Default value is 1 (ground floor)
        /// </summary>
        protected int _currentFloor = 1;

        /// <summary>
        /// Public property for occupant elevator
        /// </summary>
        public Elevator Elevator => _elevator;

        /// <summary>
        /// Weight in lbs of occupant
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// Floor that occupant is currently on. Default value is 1 (ground floor)
        /// </summary>
        public int CurrentFloor
        {
            set
            {
                if (_currentFloor != value)
                {
                    _currentFloor = value;
                    CurrentFloorChanged?.Invoke(this, new CurrentFloorChangedEventArgs(value));
                }
            }
            
            get => _currentFloor;
        }

        /// <summary>
        /// Floor that occupant wishes to ride elevator to
        /// </summary>
        public int? RequestedFloor 
        {
            private set 
            {
                if (_requestedFloor != value)
                {
                    _requestedFloor = value;
                    RequestedFloorChanged?.Invoke(this, new RequestedFloorChangedEventArgs(value)) ;
                }
            }

            get => _requestedFloor;
        }

        /// <summary>
        /// Current state of occupant
        /// </summary>
        public State CurrentState 
        {
            private set 
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    StateChanged?.Invoke(this, new StateChangedEventArgs(value));
                }
            }
            
            get => _currentState;
        }

        /// <summary>
        /// State change event
        /// </summary>
        public delegate void StateChangedHandler(Occupant sender, StateChangedEventArgs eventArgs);
        public event StateChangedHandler StateChanged;
        public class StateChangedEventArgs : EventArgs
        {
            public State State { get; }

            public StateChangedEventArgs(State state)
            {
                State = state;
            }
        }

        /// <summary>
        /// Requested floor changed event
        /// </summary>
        public delegate void RequestedFloorChangedHandler(Occupant sender, RequestedFloorChangedEventArgs eventArgs);
        public event RequestedFloorChangedHandler RequestedFloorChanged;
        public class RequestedFloorChangedEventArgs : EventArgs
        {
            public int? FloorNumber { get; }

            public RequestedFloorChangedEventArgs(int? floorNumber)
            {
                FloorNumber = floorNumber;
            }
        }

        /// <summary>
        /// Current floor changed event
        /// </summary>
        public delegate void CurrentFloorChangedHandler(Occupant sender, CurrentFloorChangedEventArgs eventArgs);
        public event CurrentFloorChangedHandler CurrentFloorChanged;
        public class CurrentFloorChangedEventArgs : EventArgs
        {
            public int? FloorNumber { get; }

            public CurrentFloorChangedEventArgs(int? floorNumber)
            {
                FloorNumber = floorNumber;
            }
        }

        public Occupant() { }

        /// <param name="weight">Weight of occupant</param>
        public Occupant(Building building, double weight)
        {
            if (building == null)
            {
                throw new ArgumentNullException(nameof(building), "Building cannot be null.");
            }

            if (weight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), weight, $"{weight} is not a valid weight.");
            }

            _building = building;
            Weight = weight;

            building.Occupants.Add(this);

            Id = ++_occupantCount;
        }

        /// <summary>
        /// Sends a request for an elevator
        /// </summary>
        /// <param name="direction">Desired direction of travel</param>
        public Task RequestElevatorAsync(Elevator.Direction direction)
        {
            if (direction != Elevator.Direction.Up && direction != Elevator.Direction.Down)
            {
                throw new ArgumentException($"{direction} is not a valid direction");
            }

            _requestedDirection = direction;
            CurrentState = State.waiting;

            foreach (var elevator in _building.Elevators)
            {
                elevator.StateChanged += HandleElevatorStateChanged;
            }

            return _building.RequestAsync(this, direction);
        }

        /// <summary>
        /// Sends request to elevator to stop at floor
        /// </summary>
        /// <param name="floorNumber">Desired floor's number</param>
        public Task RequestFloorAsync(int floorNumber)
        {
            if (Elevator == null)
            {
                throw new Exception("Occupant is not in elevator");
            }

            if (floorNumber < 1 || floorNumber > _building.FloorCount)
            {
                throw new ArgumentOutOfRangeException();
            }

            RequestedFloor = floorNumber;
            return Elevator.AddDisembarkRequestAsync(new DisembarkRequest(floorNumber));
        }

        /// <summary>
        /// Notify the occupant the elevator is ready. Enter the elevator if valid.
        /// </summary>
        /// <param name="elevator">Notifying elevator</param>
        public async Task NotifyElevatorReadyAsync(Elevator elevator)
        {
            if (elevator.CanEnter(this))
            {
                await elevator.EnterAsync(this);
                elevator.StateChanged += HandleElevatorStateChanged;
                _elevator = elevator;
                CurrentState = State.riding;
            }
        }

        /// <summary>
        /// Event handler for elevator state change. Exit the elevator if the destination floor has been reached.
        /// </summary>
        /// <param name="elevator">Elevator of occupant</param>
        /// <param name="eventArgs">Event arguments</param>
        public void HandleElevatorStateChanged(Elevator elevator, Elevator.StateChangedEventArgs eventArgs)
        {
            if (eventArgs.NewState == Elevator.State.DoorsOpen)
            {
                if (CurrentState == State.waiting && elevator.CurrentFloor == CurrentFloor)
                {
                    elevator.EnterAsync(this).Wait();
                    CurrentState = State.riding;
                }
                else if (CurrentState == State.riding && elevator.CurrentFloor == RequestedFloor)
                {
                    elevator.ExitAsync(this);

                    foreach (var item in _building.Elevators)
                    {
                        item.StateChanged -= HandleElevatorStateChanged;
                    }
                    
                    RequestedFloor = null;
                    _elevator = null;
                    CurrentState = State.none;
                    CurrentFloor = elevator.CurrentFloor;
                }
            }
        }

        #region Occupant Enums

        public enum State { none, waiting, riding }

        #endregion
    }
}