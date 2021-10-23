using System;
using ElevatorApp.Core;
using Microsoft.AspNetCore.SignalR.Client;

namespace ElevatorApp.Client
{
    public class App
    {
        HubConnection _hubConnection;

        public App(int port)
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"http://localhost:{port}{HubConstants.URL_PATH}")
                    .Build();

                _hubConnection.StartAsync().Wait();

                Console.WriteLine("Connection successful");

                _hubConnection.On<Occupant>(HubConstants.RECEIVE_OCCUPANT, occupant =>
                {
                    Console.WriteLine(occupant.Id);
                });

                _hubConnection.On<Elevator>(HubConstants.RECEIEVE_ELEVATOR_UPDATE, elevator =>
                {
                    Console.WriteLine(elevator);
                });

                var occupant = _hubConnection.InvokeAsync<Occupant>(HubConstants.CREATE_OCCUPANT).Result;

            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                Console.WriteLine($"An error occured while connecting to the server: {ex.Message}");
            }
        }
    }
}