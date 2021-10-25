using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class ElvatorTests
    {
        private static readonly MockFactory factory = new MockFactory();

        [Fact]
        public void Min_Weight_Cannot_Be_Zero()
        {
            TestBadConstructorArguments(0);
        }

        [Fact]
        public void Min_Weight_Cannot_Be_Negative()
        {
            TestBadConstructorArguments(-0.1);
        }

        void TestBadConstructorArguments(double maxElevatorWeight)
        {
            // Given
            bool isConstructFailed = false;
            var building = new Building(2, 1, 1);

            try
            {
                // When
                _ = new Elevator(building, maxElevatorWeight);
            }
            catch (ArgumentOutOfRangeException)
            {
                isConstructFailed = true;
            }
            finally
            {
                // Then
                Assert.True(isConstructFailed, $"Test failed with {nameof(maxElevatorWeight)}: {maxElevatorWeight}");
            }
        }

        [Fact]
        public async void Occupant_Can_Enter_Stopped_Elevator_On_Same_Floor()
        {
            var occupant = factory.CreateOccupant();
            await TestIsInElevatorAsync(occupant);
        }

        [Fact]
        public async void Occupant_Can_Request_Floor_Inside_Elevator()
        {
            var occupant = factory.CreateOccupant();
            int floorNumber = 2;

            await TestElevatorIsMovingAsync(occupant, floorNumber);
        }

        [Fact]
        public async void Occupant_Exits_Elevator_On_Requested_Floor()
        {
            var occupant = factory.CreateOccupant();
            int floorNumber = 2;
            
            await TestOccupantExitsOnRequestedFloor(occupant, floorNumber);
        }

        private async Task TestIsInElevatorAsync(Occupant occupant)
        {
            // Given
            bool isInElevator = false;
            Occupant.StateChangedHandler onStateChanged = (Occupant occupant, Occupant.StateChangedEventArgs eventArgs) => 
            {
                isInElevator = eventArgs.State == Occupant.State.riding && occupant.Elevator != null;
            };
            occupant.StateChanged += onStateChanged;

            // When
            await occupant.RequestElevatorAsync(Elevator.Direction.Up);

            Thread.Sleep(ElevatorConstants.ELAPSE_TIME * 10 + 1000);

            // Then
            Assert.True(isInElevator, $"Occupant did not board the elevator - State: {occupant.CurrentState}, In elevator: {occupant.Elevator != null}");

            occupant.StateChanged -= onStateChanged;
        }

        private async Task TestElevatorIsMovingAsync(Occupant occupant, int floorNumber)
        {
            // Given
            bool isElevatorMoving = false;
            Elevator.StateChangedHandler onElevatorStateChanged = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) => 
            {
                isElevatorMoving = elevator.CurrentState == Elevator.State.Moving;
            };

            // When
            await TestIsInElevatorAsync(occupant);
            occupant.Elevator.StateChanged += onElevatorStateChanged;
            await occupant.RequestFloorAsync(floorNumber);
            Thread.Sleep(ElevatorConstants.TIME_TO_CLOSE_DOORS * 1000 + ElevatorConstants.ELAPSE_TIME);

            // Then
            Assert.True(isElevatorMoving, "The occupant's request was not acknowledged.");

            occupant.Elevator.StateChanged -= onElevatorStateChanged;
        }

        private async Task TestOccupantExitsOnRequestedFloor(Occupant occupant, int floorNumber)
        {
            // Given
            bool isElevatorStopped = false;
            bool isElevatorEmpty = false;
            bool isOccupantOutsideElevator = false;

            Elevator.StateChangedHandler onElevatorStateChanged1 = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) => 
            {
                isElevatorStopped = elevator.CurrentState != Elevator.State.Moving;
            };
            Elevator.StateChangedHandler onElevatorStateChanged2 = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) => 
            {
                isElevatorEmpty = elevator.OccupantsCount == 0;
            };
            Occupant.StateChangedHandler onOccupantStateChanged = (Occupant occupant, Occupant.StateChangedEventArgs eventArgs) => 
            {
                isOccupantOutsideElevator = occupant.CurrentFloor == floorNumber && occupant.CurrentState == Occupant.State.none && occupant.Elevator == null;
            };

            // When
            await TestIsInElevatorAsync(occupant);
            await TestElevatorIsMovingAsync(occupant,floorNumber);
            occupant.StateChanged += onOccupantStateChanged;
            occupant.Elevator.StateChanged += onElevatorStateChanged1;
            occupant.Elevator.StateChanged += onElevatorStateChanged2;

            Thread.Sleep((floorNumber - 1) * Convert.ToInt32(ElevatorConstants.FLOOR_HEIGHT) / ElevatorConstants.MAX_SPEED + ElevatorConstants.ELAPSE_TIME);

            // Then
            Assert.True(isElevatorStopped, "Elevator is not stopped");
            Assert.True(isElevatorEmpty, "Elevator is not empty");
            Assert.True(isOccupantOutsideElevator, "Occupant is still inside of elevator.");

            occupant.StateChanged -= onOccupantStateChanged;
            occupant.Elevator.StateChanged -= onElevatorStateChanged1;
            occupant.Elevator.StateChanged -= onElevatorStateChanged2;
        }
    }
}