using System;
using System.Timers;

namespace ElevatorApp.Core
{
    /// <summary>
    /// Contains the events for Eelvator
    /// </summary>
    public partial class Elevator
    {
        /// <summary>
        /// State change event
        /// </summary>
        public delegate void StateChangedHandler(StateChangedEventArgs eventArgs);
        public event StateChangedHandler StateChanged;
        public class StateChangedEventArgs : EventArgs
        {
            public State State { get; }

            public StateChangedEventArgs(State state)
            {
                State = state;
            }
        }
        protected virtual void OnStateChanged(StateChangedEventArgs eventArgs)
        {
            var handler = StateChanged;
            handler?.Invoke(eventArgs);
        }

        /// <summary>
        /// Direction change event
        /// </summary>
        public delegate void DirectionChangedHandler(DirectionChangedEventArgs eventArgs);
        public event DirectionChangedHandler DirectionChanged;
        public class DirectionChangedEventArgs : EventArgs
        {
            public Direction Direction { get; }

            public DirectionChangedEventArgs(Direction direction)
            {
                Direction = direction;
            }
        }
        protected virtual void OnDirectionChanged(DirectionChangedEventArgs eventArgs)
        {
            var handler = DirectionChanged;
            handler?.Invoke(eventArgs);
        }

        /// <summary>
        /// Floor change event
        /// </summary>
        public delegate void FloorChangedHandler(FloorChangedEventArgs eventArgs);
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
            handler?.Invoke(eventArgs);
        }

        private void OnTimeElapsed(object source, ElapsedEventArgs eventArgs)
        {
            if (IsMoving)
            {
                if (IsAtDestinationFloor)
                {
                    SetCurrentStateAsync(State.Ready).Wait();
                    OpenDoorsAsync().Wait();
                }
                else if (IsDirectionUp)
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
            }
            // CONDITION: Doors are closed
            else if (IsDoorsClosed)
            {
                SetNextRequestAsync().Wait();
            }
        }
    }
}