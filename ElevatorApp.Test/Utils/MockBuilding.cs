using System;
using System.IO;
using ElevatorApp.Core;
using Microsoft.Extensions.Logging;

namespace ElevatorApp.Test
{
    public class MockBuilding : Building
    {
        private static readonly string _logFilePath = Path.Combine(Environment.CurrentDirectory, "log.txt");
        private readonly DateTime StartTime = DateTime.Now;

        public MockBuilding(int floorCount, int elevatorCount, double maxElevatorWeight) 
            : base(floorCount, elevatorCount, maxElevatorWeight)
        {
            // Create file for logging
            using (var writer = File.Create(_logFilePath))
            {
                 writer.Close();
            }
        }

        public override void LogMessage(string message, LogLevel level = LogLevel.Information)
        {
            base.LogMessage(message, level);
            
            message = $"{(DateTime.Now - StartTime).ToString()} | {message}";
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                 writer.WriteLine(message);
            }
        }
    }
}