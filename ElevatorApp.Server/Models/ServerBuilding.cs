using ElevatorApp.Core;
using Microsoft.Extensions.Configuration;

namespace ElevatorApp.Server.Models
{
    /// <summary>
    /// Same as original buildling class. Constructor arguments are retrieved from appsettings.json
    /// </summary>
    public class ServerBuilding : Building
    {
        public ServerBuilding(IConfiguration config) 
            : base(
                config.GetValue<int>("Building:FloorCount"), 
                config.GetValue<int>("Building:ElevatorCount"), 
                config.GetValue<int>("Building:MaxElevatorWeight"))
        { }
    }
}