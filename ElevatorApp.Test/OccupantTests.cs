using System;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class OccupantTests
    {
        public static readonly MockFactory factory = new MockFactory();

        /// <summary>
        /// Occupant weight must be greater than zero
        /// </summary>
        [Fact]
        public void Weight_Cannot_Be_Zero()
        {
            TestBadConstructorArguments(0);
        }

        /// <summary>
        /// Occupant weight must be greater than zero
        /// </summary>
        [Fact]
        public void Weight_Cannot_Be_Negative()
        {
            TestBadConstructorArguments(-0.1);
        }

        /// <summary>
        /// Passes if invalid arguments throw an error
        /// </summary>
        /// <param name="weight">Occupant's weight</param>
        void TestBadConstructorArguments(double weight)
        {
            // Given
            bool isConstructFailed = false;

            try
            {
                // When
                _ = new Occupant(factory.CreateBuilding(), weight);
            }
            catch (ArgumentOutOfRangeException)
            {
                isConstructFailed = true;
            }
            finally
            {
                // Then
                Assert.True(isConstructFailed, $"Test failed with {nameof(weight)}: {weight}");
            }
        }
    }
}