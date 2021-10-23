using System;
using ElevatorApp.Core;
using Xunit;

namespace ElevatorApp.Test
{
    public class OccupantTests
    {
        public static readonly MockFactory factory = new MockFactory();

        [Fact]
        public void Weight_Cannot_Be_Zero()
        {
            TestBadConstructorArguments(0);
        }

        [Fact]
        public void Weight_Cannot_Be_Negative()
        {
            TestBadConstructorArguments(-0.1);
        }

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