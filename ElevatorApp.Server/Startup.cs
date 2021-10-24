using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorApp.Core;
using ElevatorApp.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ElevatorApp.Server
{
    public class Startup
    {
        private readonly string _corsOrigin = "corsOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: _corsOrigin, builder => {
                    builder.SetIsOriginAllowed(x => _ = true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .Build();
                });
            });

            services.AddSignalR();

            // Add an instance of Building as an injectable dependnecy
            services.AddSingleton(CreateBuilding());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(_corsOrigin);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ElevatorHub>(HubConstants.URL_PATH);
            });
        }

        /// <summary>
        /// Creates a building for the application
        /// </summary>
        private Building CreateBuilding()
        {
#if DEBUG
            return new Building(10, 2, 15000);
#else
            int floorCount = GetIntFromuser("Enter number of building floors", 2);
            int elevatorCount = GetIntFromuser("Enter number of elevators in building");
            int maxElevatorWeight = GetIntFromuser("Enter elevator weight capacity", 1000);
#endif
        }

        /// <summary>
        /// Fetches and validates user input for an integer value
        /// </summary>
        private int GetIntFromuser(string prompt, int minimumValue = 1)
        {
            while (true)
            {
                Console.ResetColor();
                Console.Write($"{prompt}: ");

                if (int.TryParse(Console.ReadLine(), out int input))
                {
                    if (input >= minimumValue)
                    {
                        return input;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Please enter a value greater than {minimumValue}");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter a valid integer");
                }
            }
        }
    }
}
