using System;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class BuildingTests
    {
        #region Constructor Tests

        /// <summary>
        /// Floor count must be greater than one
        /// </summary>
        [Fact]
        public void FloorCount_Cannot_Be_One()
        {
            TestBadConstructorArguments(1, 1, 1);
        }

        /// <summary>
        /// Floor count must be greater than one
        /// </summary>
        [Fact]
        public void FloorCount_Cannot_Be_Less_Than_1()
        {
            TestBadConstructorArguments(0, 1, 1);
        }

        /// <summary>
        /// Elevator count must be at least one
        /// </summary>
        [Fact]
        public void Elevator_Count_Cannot_Be_Less_Than_1()
        {
            TestBadConstructorArguments(2, 0, 1);
        }

        /// <summary>
        /// Elevator max weight must be greater than zero
        /// </summary>
        [Fact]
        public void MaxElevatorWeight_Cannot_Be_Zero()
        {
            TestBadConstructorArguments(2, 1, 0);
        }

        /// <summary>
        /// Elevator max weight must be greater than zero
        /// </summary>
        [Fact]
        public void MaxElevatorWeight_Cannot_Be_Less_Than_Zero()
        {
            TestBadConstructorArguments(2, 1, -0.1);
        }

        /// <summary>
        /// An error should be thrown if invalid arguments are provided to the constructor
        /// </summary>
        /// <param name="floorCount">Number of floors in building</param>
        /// <param name="elevatorCount">Number of elevators in building</param>
        /// <param name="maxElevatorWeight">Max weight capacity for elevator</param>
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