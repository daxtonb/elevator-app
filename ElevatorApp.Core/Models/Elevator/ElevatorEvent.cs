using System;
using System.Timers;

namespace ElevatorApp.Core
{
    /// <summary>
    /// This partial class contains the events for Eelvator
    /// </summary>
    public partial class Elevator
    {
        private bool _isWorking;

        /// <summary>
        /// State change event
        /// </summary>
        public delegate void StateChangedHandler(Elevator sender, StateChangedEventArgs eventArgs);
        public event StateChangedHandler StateChanged;
        public class StateChangedEventArgs : EventArgs
        {
            public State PreviousState { get; }
            public State NewState { get; }

            public StateChangedEventArgs(State previousState, State newState)
            {
                PreviousState = previousState;
                NewState = newState;
            }
        }

        /// <summary>
        /// Direction change event
        /// </summary>
        public delegate void DirectionChangedHandler(Elevator elevator, DirectionChangedEventArgs eventArgs);
        public event DirectionChangedHandler DirectionChanged;
        public class DirectionChangedEventArgs : EventArgs
        {
            public Direction Direction { get; }

            public DirectionChangedEventArgs(Direction direction)
            {
                Direction = direction;
            }
        }

        /// <summary>
        /// Floor change event
        /// </summary>
        public delegate void FloorChangedHandler(Elevator elevator, FloorChangedEventArgs eventArgs);
        public event FloorChangedHandler FloorChanged;
        public class FloorChangedEventArgs : EventArgs
        {
            public int FloorNumber { get; }

            public FloorChangedEventArgs(int floorNumber)
            {
                FloorNumber = floorNumber;
            }
        }
        protected virtual void OnFloorChanged(FloorChangedEventArgs eventArgs)
        {
            var handler = FloorChanged;
            handler?.Invoke(this, eventArgs);

            // Update all of the occupants
            foreach (var occupant in _occupants)
            {
                occupant.CurrentFloor = eventArgs.FloorNumber;
            }
        }

        /// <summary>
        /// Request made event
        /// </summary>
        public delegate void RequestMadeHandler(Elevator elevator, RequestMadeEventArgs eventArgs);
        public event RequestMadeHandler RequestMade;
        public class RequestMadeEventArgs : EventArgs
        {
            public Request Request { get; }

            public RequestMadeEventArgs(Request request)
            {
                Request = request;
            }
        }
        protected virtual void OnRequestMade(RequestMadeEventArgs eventArgs)
        {
            var handler = RequestMade;
            handler?.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Logic for updating elevator state, direction, and floor
        /// </summary>
        private void OnTimeElapsed(object source, ElapsedEventArgs eventArgs)
        {
            if (_isWorking)
            {
                return;
            }

            _isWorking = true;

            if (IsAtDestinationFloor && _doorsOpenedDateTime == null && _currentRequest != null)
            {
                CurrentState = State.Ready;

                OpenDoors();
                RemoveRequest<DisembarkRequest>(CurrentFloor);

                _currentRequest = null;
            }
            else if (IsMoving)
            {
                if (IsDirectionUp)
                {
                    _currentHeight += (_maxSpeed * ((double)ElevatorConstants.ELAPSE_TIME / 1000));
                }
                else
                {
                    _currentHeight -= (_maxSpeed * ((double)ElevatorConstants.ELAPSE_TIME / 1000));
                }

                                _building.LogMessage($"{_currentHeight}\t{_building.FloorHeight}\t{_currentHeight % _building.FloorHeight}");

                // CONDITION: Floor was changed
                if (_currentHeight % _building.FloorHeight == 0)
                {
                    OnFloorChanged(new FloorChangedEventArgs(CurrentFloor));
                }
            }

            // CONDITION: Doors have been open for the maximum allowed time
            else if (IsDoorsOpen && _doorsOpenedDateTime.HasValue
                && DateTime.UtcNow.AddSeconds(-_timeToCloseDoors) >= _doorsOpenedDateTime.Value)
            {
                CloseDoorsAsync().Wait();
                _doorsOpenedDateTime = null;
            }
            // CONDITION: Doors are closed
            else if (IsDoorsClosed || IsReady)
            {
                SetNextRequestAsync().Wait();

                if (_currentRequest != null)
                {
                    CurrentState = State.Moving;
                }
                else if (OccupantsCount == 0)
                {
                    CurrentState = State.Ready;
                    CurrentDirection = Direction.None;
                }
            }

            _isWorking = false;
        }
    }
}