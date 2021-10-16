using System;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class BuildingTests
    {
        #region Constructor Tests
        [Fact]
        public void FloorCount_Cannot_Be_One()
        {
            TestBadConstructorArguments(1, 1, 1);
        }

        [Fact]
        public void FloorCount_Cannot_Be_Less_Than_1()
        {
            TestBadConstructorArguments(0, 1, 1);
        }

        [Fact]
        public void Elevator_Count_Cannot_Be_Less_Than_1()
        {
            TestBadConstructorArguments(2, 0, 1);
        }

        [Fact]
        public void MaxElevatorWeight_Cannot_Be_Zero()
        {
            TestBadConstructorArguments(2, 1, 0);
        }

        [Fact]
        public void MaxElevatorWeight_Cannot_Be_Less_Than_Zero()
        {
            TestBadConstructorArguments(2, 1, -0.1);
        }

        void TestBadConstructorArguments(int floorCount, int elevatorCount, double maxElevatorWeight)
        {
            // Given
            bool isConstructFailed = false;

            try
            {
                // When
                _ = new Building(floorCount, elevatorCount, maxElevatorWeight);
            }
            catch (ArgumentOutOfRangeException)
            {
                isConstructFailed = true;
            }
            finally
            {
                // Then
                Assert.True(isConstructFailed, $"Test failed with {nameof(floorCount)}: {floorCount}, {nameof(elevatorCount)}: {elevatorCount}, {nameof(maxElevatorWeight)}: {maxElevatorWeight}");
            }
        }

        #endregion
    }
}