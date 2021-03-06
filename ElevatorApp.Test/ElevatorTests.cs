using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class ElvatorTests
    {
        private static readonly MockFactory factory = new MockFactory();

        /// <summary>
        /// Weight must be greater than zero
        /// </summary>
        [Fact]
        public void Min_Weight_Cannot_Be_Zero()
        {
            TestBadConstructorArguments(0);
        }

        /// <summary>
        /// Weight must be positive number
        /// </summary>
        [Fact]
        public void Min_Weight_Cannot_Be_Negative()
        {
            TestBadConstructorArguments(-0.1);
        }

        /// <summary>
        /// Invalid Elevator constructor arguments should fail
        /// </summary>
        /// <param name="maxElevatorWeight">Max elevator weight</param>
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

        /// <summary>
        /// An occupant should be able to board an available elevator
        /// </summary>
        [Fact]
        public async void Occupant_Can_Board_Elevator()
        {
            var occupant = factory.CreateOccupant();
            await TestIsInElevatorAsync(occupant);
        }

        /// <summary>
        /// An occupant inside an elevator should be able to request a floor
        /// </summary>
        [Fact]
        public async void Occupant_Can_Request_Floor_Inside_Elevator()
        {
            var occupant = factory.CreateOccupant();
            int floorNumber = 2;

            await TestElevatorIsMovingAsync(occupant, floorNumber);
        }

        /// <summary>
        /// An occupant should exit elevator once destination floor is reached
        /// </summary>
        [Fact]
        public async void Occupant_Exits_Elevator_On_Requested_Floor()
        {
            var occupant = factory.CreateOccupant();
            int floorNumber = 2;
            
            await TestOccupantExitsOnRequestedFloor(occupant, floorNumber);
        }

        /// <summary>
        /// An elevator should make stops at floor in sequence regardless of the order requests were submitted
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async void Elevator_Stops_In_Sequence_Of_Direction()
        {
            // Given
            var occupant = factory.CreateOccupant();
            int floor1 = 5, floor2 = 3;
            int? stop1 = null, stop2 = null;

            Elevator.StateChangedHandler onElevatorStateChanged = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) =>
            {
                if (eventArgs.NewState == Elevator.State.Ready)
                {
                    if (stop1 == null)
                        stop1 = elevator.CurrentFloor;
                    else
                        stop2 = elevator.CurrentFloor;
                }
            };

            // When
            await TestIsInElevatorAsync(occupant);
            occupant.Elevator.StateChanged += onElevatorStateChanged;
            await occupant.RequestFloorAsync(floor1);
            await occupant.RequestFloorAsync(floor2);
            
            Thread.Sleep(Convert.ToInt32((((ElevatorConstants.FLOOR_HEIGHT / ElevatorConstants.MAX_SPEED) * floor1)+ ElevatorConstants.TIME_TO_CLOSE_DOORS) * 1000));
            
            // Then
            Assert.NotNull(stop1);
            Assert.NotNull(stop2);
            Assert.Equal(stop1, floor2);
            Assert.Equal(stop2, floor1);
        }

        /// <summary>
        /// Elevator wills top at floor that is along the way to original destination floor
        /// </summary>
        [Fact]
        public async void Elevator_Stops_For_Intermittent_Floor_Request()
        {
            // Given
            var occupant1 = factory.CreateOccupant();
            var occupant2 = factory.CreateOccupant((occupant1 as MockOccupant).Building, 3);
            int floor = 5;
            int? stop1 = null, stop2 = null;

            Elevator.StateChangedHandler onElevatorStateChanged = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) =>
            {
                if (eventArgs.NewState == Elevator.State.Ready)
                {
                    if (stop1 == null)
                        stop1 = elevator.CurrentFloor;
                    else
                        stop2 = elevator.CurrentFloor;
                }
            };

            // When
            await RequestElevatorAsync(occupant1, Elevator.Direction.Up);
            occupant1.Elevator.StateChanged += onElevatorStateChanged;

            await RequestFloorAsync(occupant1, floor);
            await RequestElevatorAsync(occupant2, Elevator.Direction.Up);
            Thread.Sleep(Convert.ToInt32((((ElevatorConstants.FLOOR_HEIGHT / ElevatorConstants.MAX_SPEED) * floor) + (ElevatorConstants.TIME_TO_CLOSE_DOORS)) * 1000));

            // Then
            Assert.True(stop1 < stop2, $"stop1: {stop1}, stop2: {stop2}");
        }

        /// <summary>
        /// If user selects a floor while the elevator is moving, and the selected floor is the next floor
        /// that the elevator is about to pass, the elevator will skip this request and come back to it later
        /// </summary>
        [Fact]
        public async void Elevator_Skips_Request_For_Next_Floor()
        {
            // Given
            var occupant = factory.CreateOccupant();
            int floor = 4;
            int floorToSkip = floor - 1;
            int floorToRequestFrom = floor - 2;
            int? stop1 = null, stop2 = null;

            Elevator.FloorChangedHandler onElevatorFloorChanged = (Elevator elevator, Elevator.FloorChangedEventArgs eventArgs) =>
            {
                // Request 
                if (eventArgs.FloorNumber == floorToRequestFrom)
                {
                    occupant.RequestFloorAsync(floorToSkip).Wait();
                }
            };
            Elevator.StateChangedHandler onElevatorStateChanged = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) =>
            {
                if (eventArgs.NewState == Elevator.State.Ready)
                {
                    if (stop1 == null)
                        stop1 = elevator.CurrentFloor;
                    else
                        stop2 = elevator.CurrentFloor;
                }
            };

            // When
            await RequestElevatorAsync(occupant, Elevator.Direction.Up);
            occupant.Elevator.FloorChanged += onElevatorFloorChanged;
            occupant.Elevator.StateChanged += onElevatorStateChanged;
            await RequestFloorAsync(occupant, floor);
            Thread.Sleep(Convert.ToInt32((((ElevatorConstants.FLOOR_HEIGHT / ElevatorConstants.MAX_SPEED) * (floor - floorToSkip + floor)) + (ElevatorConstants.TIME_TO_CLOSE_DOORS * 2)) * 1000));

            // Then
            Assert.NotNull(stop1);
            Assert.NotNull(stop2);
            Assert.True(stop1 > stop2, $"stop1: {stop1}, stop2: {stop2}");
        }

        /// <summary>
        /// An elevator that cannot fit the requesting occupant will return to occupant after an occupant exits
        /// </summary>
        [Fact]
        public async void Full_Elevator_Returns()
        {
            // Given
            var building = factory.CreateBuilding();
            var occupantWaiting = factory.CreateOccupant(building);
            var occupants = new List<Occupant>();
            int floorNumber = 2;
            // Fill up all of the elevators as much as possible
            do
            {
                var occupant = factory.CreateOccupant(building);
                await RequestElevatorAsync(occupant, Elevator.Direction.Up);
                occupants.Add(occupant);
            } while (building.Elevators.Count(e => e.Capcity == 100) != building.Elevators.Length);

            var occupantInElevator = occupants.First();

            // When

            // Have waiting occupant place an elevator request
            await RequestElevatorAsync(occupantWaiting, Elevator.Direction.Up);
            // Have occupant inside elevator request a floor
            await RequestFloorAsync(occupantInElevator, floorNumber);
            Thread.Sleep(Convert.ToInt32((ElevatorConstants.FLOOR_HEIGHT / ElevatorConstants.MAX_SPEED) * (floorNumber * 2) + (ElevatorConstants.TIME_TO_CLOSE_DOORS)) * 1000);
            
            // Then
            Assert.NotNull(occupantWaiting.Elevator);
        }

        /// <summary>
        /// Simulates a user requesting and entering an elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        /// <param name="direction">Direction of travel</param>
        private async Task RequestElevatorAsync(Occupant occupant, Elevator.Direction direction)
        {
            await occupant.RequestElevatorAsync(Elevator.Direction.Up);

            Thread.Sleep(ElevatorConstants.ELAPSE_TIME + 1000);
        }

        /// <summary>
        /// Simulates a user requesting a floor from inside an elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        /// <param name="floorNumber">Requested floor</param>
        private async Task RequestFloorAsync(Occupant occupant, int floorNumber)
        {
            await occupant.RequestFloorAsync(floorNumber);

            Thread.Sleep(ElevatorConstants.TIME_TO_CLOSE_DOORS * 1000 + ElevatorConstants.ELAPSE_TIME);
        }

        /// <summary>
        /// Tests whether the occupant was able to enter requested elevator
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
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
            await RequestElevatorAsync(occupant, Elevator.Direction.Up);

            // Then
            Assert.True(isInElevator, $"Occupant did not board the elevator - State: {occupant.CurrentState}, In elevator: {occupant.Elevator != null}");

            occupant.StateChanged -= onStateChanged;
        }

        /// <summary>
        /// Tests whether the elevator moves after a request is made
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        /// <param name="floorNumber">Requested floor's number</param>
        private async Task TestElevatorIsMovingAsync(Occupant occupant, int floorNumber)
        {
            // Given
            bool isElevatorMoving = false;
            Elevator.StateChangedHandler onElevatorStateChanged = (Elevator elevator, Elevator.StateChangedEventArgs eventArgs) => 
            {
                isElevatorMoving = eventArgs.NewState == Elevator.State.Moving;
            };

            // When
            await TestIsInElevatorAsync(occupant);
            occupant.Elevator.StateChanged += onElevatorStateChanged;
            await RequestFloorAsync(occupant, floorNumber);

            // Then
            Assert.True(isElevatorMoving, "The occupant's request was not acknowledged.");

            occupant.Elevator.StateChanged -= onElevatorStateChanged;
        }

        /// <summary>
        /// Tests that the elevator travels to the requested floor and the occupant exits on requested floor
        /// </summary>
        /// <param name="occupant">Requesting occupant</param>
        /// <param name="floorNumber">Requested floor's number</param>
        /// <returns></returns>
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
            await TestElevatorIsMovingAsync(occupant,floorNumber);
            occupant.StateChanged += onOccupantStateChanged;
            occupant.Elevator.StateChanged += onElevatorStateChanged1;
            occupant.Elevator.StateChanged += onElevatorStateChanged2;

            Thread.Sleep(Convert.ToInt32(((ElevatorConstants.FLOOR_HEIGHT / ElevatorConstants.MAX_SPEED) + ElevatorConstants.TIME_TO_CLOSE_DOORS) * 1000) + ElevatorConstants.ELAPSE_TIME);

            // Then
            Assert.True(isElevatorStopped, "Elevator is not stopped");
            Assert.True(isElevatorEmpty, "Elevator is not empty");
            Assert.True(isOccupantOutsideElevator, "Occupant is still inside of elevator.");

            occupant.StateChanged -= onOccupantStateChanged;
        }
    }
}