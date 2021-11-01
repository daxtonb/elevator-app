using System;
using System.IO;
using ElevatorApp.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElevatorApp.Server.Models
{
    /// <summary>
    /// Same as original buildling class. Constructor arguments are retrieved from appsettings.json
    /// </summary>
    public class ServerBuilding : Building
    {
        private static readonly string _logFilePath = Path.Combine(Environment.CurrentDirectory, "log.txt");

        public ServerBuilding(IConfiguration config, ILogger<Building> logger) 
            : base(
                config.GetValue<int>("Building:FloorCount"), 
                config.GetValue<int>("Building:ElevatorCount"), 
                config.GetValue<int>("Building:MaxElevatorWeight"),
                logger)
        { 
            // Create file for logging
            using (var writer = File.Create(_logFilePath))
            {
                 writer.Close();
            }
        }

        /// <summary>
        /// Additionally logs messages to log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public override void LogMessage(string message, LogLevel level = LogLevel.Information)
        {
            base.LogMessage(message, level);

            message = $"{DateTime.Now.ToString("hh:mm:ss")} | {message}";
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                 writer.WriteLine(message);
            }
        }
    }
}