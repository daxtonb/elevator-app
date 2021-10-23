using System;
using System.Collections.Generic;

namespace ElevatorApp.Core.Utils
{
    public static class RequestHelper
    {
        public static Request GetClosestRequest<T>(IEnumerable<T> requests, Elevator elevator) where T : Request
        {
            Request closest = null;

            foreach (var current in requests)
            {
                if (current == null)
                {
                    continue;
                }
                if (closest == null)
                {
                    closest = current;
                }
                else
                {
                    if (Math.Abs(elevator.CurrentFloor - current.FloorNumber) < Math.Abs(elevator.CurrentFloor - closest.FloorNumber))
                    {
                        closest = current;
                    }
                }
            }

            return closest;
        }
    }
}