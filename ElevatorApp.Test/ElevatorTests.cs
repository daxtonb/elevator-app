using System;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class ElvatorTests
    {
        [Fact]
        public void Min_Weight_Cannot_Be_Zero()
        {
            Test_Invalid_Elevator_Min_Weight(0);
        }

        [Fact]
        public void Min_Weight_Cannot_Be_Negative()
        {
            Test_Invalid_Elevator_Min_Weight(-0.1);
        }

        void Test_Invalid_Elevator_Min_Weight(double weight)
        {
            // Arrange
            bool isFailedToConstruct = false;
            try

            {
                // Act
                _ = new Elevator(weight);
            }
            catch (ArgumentOutOfRangeException)
            {
                isFailedToConstruct = true;
            }
            finally
            {
                // Assert
                Assert.True(isFailedToConstruct, $"The Elevator class was constructed with a minimum weight of {weight}.");
            }
        }
    }
}