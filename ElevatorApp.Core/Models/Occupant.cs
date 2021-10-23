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
        private readonly Building _building;

        /// <summary>
        /// Requested direction for elevator
        /// </summary>
        private Elevator.Direction _requestedDirection;

        /// <summary>
        /// Current state of occupant
        /// </summary>
        private State _currentState;

        /// <summary>
        /// Elevator of occupant
        /// </summary>
        private Elevator _elevator;

        /// <summary>
        /// Weight in lbs of occupant
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// Floor that occupant is currently on. Default value is 1 (ground floor)
        /// </summary>
        public int CurrentFloor { get; set; } = 1;

        /// <summary>
        /// Floor that occupant wishes to ride elevator to
        /// </summary>
        public int RequestedFloor { get; set; }

        /// <summary>
        /// Current state of occupant
        /// </summary>
        public State CurrentState => _currentState;

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
            _currentState = State.waiting;

            return _building.RequestAsync(this, direction);
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
                _currentState = State.riding;
                _elevator = elevator;
            }
        }

        /// <summary>
        /// Event handler for elevator state change. Exit the elevator if the destination floor has been reached.
        /// </summary>
        /// <param name="elevator">Elevator of occupant</param>
        /// <param name="eventArgs">Event arguments</param>
        public void HandleElevatorStateChanged(Elevator elevator, Elevator.StateChangedEventArgs eventArgs)
        {
            if (eventArgs.State == Elevator.State.DoorsOpen && elevator.CurrentFloor == RequestedFloor)
            {
                elevator.ExitAsync(this);
                elevator.StateChanged -= HandleElevatorStateChanged;
                _currentState = State.none;
            }
        }

        /// <summary>
        /// Request floor for elevator to stop at.
        /// </summary>
        /// <param name="floorNumber">Reueqsted floor number</param>
        public Task RequstFloorAsync(int floorNumber)
        {
            if (_elevator == null)
            {
                throw new Exception("Cannot request floor outside of elevator.");
            }

            if (floorNumber > _building.FloorCount)
            {
                throw new ArgumentOutOfRangeException($"{floorNumber} is not a valid floor number.");
            }

            RequestedFloor = floorNumber;

            return _elevator.AddDisembarkRequestAsync(new DisembarkRequest(floorNumber));
        }


        #region Occupant Enums

        public enum State { none, waiting, riding }

        #endregion
    }
}