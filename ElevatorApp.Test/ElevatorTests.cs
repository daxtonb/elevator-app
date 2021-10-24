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
            // Given
            var occupant = factory.CreateOccupant();
            bool isInElevator = false;

            // When
            await occupant.RequestElevatorAsync(Elevator.Direction.Up);
            occupant.StateChanged += (Occupant occupant, Occupant.StateChangedEventArgs eventArgs) => 
            {
                isInElevator = eventArgs.State == Occupant.State.riding;
            };
            
            Thread.Sleep(30000);

            // Then
            Assert.True(isInElevator, "Occupant did not board the elevator");
        }
    }
}