using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElevatorApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 6)
            {
                CreateHostBuilder(args).Build().Run();
            }
            else
            {
                Console.WriteLine($"Usage: dotnet run [floorCount] [elevatorCount] [maxElevatorWeight]");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
