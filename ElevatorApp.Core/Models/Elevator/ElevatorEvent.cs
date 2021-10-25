using System;
using System.Timers;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Contains the events for Eelvator
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
            public State State { get; }

            public StateChangedEventArgs(State state)
            {
                State = state;
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
        /// Logic for updating elevator state, direction, and floor
        /// </summary>
        private void OnTimeElapsed(object source, ElapsedEventArgs eventArgs)
        {
            if (_isWorking)
            {
                return;
            }

            _isWorking = true;

            if (IsAtDestinationFloor && _doorsOpenedDateTime == null)
            {
                CurrentState = State.Ready;

                OpenDoors();

                if (_currentRequest is BoardRequest boardRequest)
                {
                    _boardRequests.RemoveAll(r => r.FloorNumber == boardRequest.FloorNumber);
                }
                if (_currentRequest is DisembarkRequest disembarkRequest)
                {
                    _disembarkRequests.RemoveAll(r => r.FloorNumber == disembarkRequest.FloorNumber);
                }

                _currentRequest = null;
            }
            else if (IsMoving)
            {

                if (IsDirectionUp)
                {
                    _currentHeight += _maxSpeed;

                    // CONDITION: Floor was changed
                    if (_currentHeight % _building.FloorHeight == 0)
                    {
                        OnFloorChanged(new FloorChangedEventArgs(CurrentFloor));
                    }
                }
                else
                {
                    _currentHeight -= _maxSpeed;

                    // CONDITION: Floor was changed
                    if (_currentHeight % _building.FloorHeight == 0)
                    {
                        OnFloorChanged(new FloorChangedEventArgs(CurrentFloor));
                    }
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