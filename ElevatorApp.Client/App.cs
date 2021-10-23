using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElevatorApp.Core;
using Microsoft.AspNetCore.SignalR.Client;

namespace ElevatorApp.Client
{
    public class App : IDisposable
    {
        private readonly HubConnection _hubConnection;
        private BuildingViewModel _building;
        private OccupantViewModel _occupant;
        private Dictionary<int, ElevatorViewModel> _elevatorsById;
        private string _userInput;

        public App(int port)
        {
            _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"http://localhost:{port}{HubConstants.URL_PATH}")
                    .Build();

            Connect();
            StartAsync().Wait();
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        private void Connect()
        {
            try
            {
                _hubConnection.On<OccupantViewModel>(HubConstants.RECEIVE_OCCUPANT, occupant =>
                {
                    _occupant = occupant;
                });

                _hubConnection.On<BuildingViewModel>(HubConstants.RECEIVE_BUILDING, building =>
                {
                    _building = building;
                });

                Console.WriteLine("Connecting...");

                _hubConnection.StartAsync().Wait();

                // Wait for server to respond with occupant and building
                while (_occupant == null || _building == null)
                {
                    Thread.Sleep(100);
                }

                _elevatorsById = _hubConnection
                                .InvokeAsync<IEnumerable<ElevatorViewModel>>("RequestElevators")
                                .Result
                                .ToDictionary(e => e.Id);

                Console.WriteLine("Connection successful");

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

        public Task StartAsync()
        {
            bool isGetUserInput = false;

            Console.Clear();
            Task.Run(() =>
            {
                while (true)
                {
                    if (!isGetUserInput)
                    {
                        DisplayElevatorData();
                        DisplayOccupantData();
                        DisplayUserInputPrompt();
                        Thread.Sleep(1000);
                    }
                }
            });

            while (true)
            {
                if (!isGetUserInput)
                {

                    isGetUserInput = Console.ReadKey().Key.ToString() == "Enter";
                }
                else
                {
                    GetUserInput();
                }
            }
        }

        private void DisplayUserInputPrompt()
        {
            if (_occupant.CurrentState != Occupant.State.waiting)
            {
                ConsoleWriteAt("Press ENTER to perform an action.", _elevatorsById.Count + 5, 0, ConsoleColor.White, ConsoleColor.Blue);
            }
        }

        private void GetUserInput()
        {
            string text;
            Func<string, bool> validate;

            if (_occupant.CurrentState == Occupant.State.none)
            {
                if (_occupant.CurrentFloor == 1)
                {
                    text = "Enter U to go up";
                    validate = (string input) => input.ToLower() == "u";
                }
                else if (_occupant.CurrentFloor == _building.FloorCount)
                {
                    text = "Enter D to go down: ";
                    validate = (string input) => input.ToLower() == "d";
                }
                else
                {
                    text = "Enter U to go up or D to go down: ";
                    validate = (string input) => input.ToLower() == "u" || input.ToLower() == "d";
                }
            }
            else if (_occupant.CurrentState == Occupant.State.riding)
            {
                text = $"Enter floor number (1-{_building.FloorCount})";
                validate = (string input) => int.TryParse(input, out int result) && result > 0 && result <= _building.FloorCount;
            }
            else
            {
                throw new Exception("No user action to take!");
            }

            Console.Clear();
            ConsoleWriteAt(text, 0, 0);

            while (true)
            {
                string input = Console.ReadLine();
                if (validate(input))
                {
                    break;
                }
            }
        }

        private void DisplayOccupantData()
        {
            ConsoleWriteAt("YOU", _elevatorsById.Count + 2, 0, ConsoleColor.Black, ConsoleColor.White);
            ConsoleWriteAt($"Floor: {_occupant.CurrentFloor}\tStatus: {_occupant.CurrentState}", _elevatorsById.Count + 3, 0);
        }

        private void DisplayElevatorData()
        {
            ConsoleWriteAt("ELEVATORS", 0, 0, ConsoleColor.Black, ConsoleColor.White);
            foreach (var elevator in _elevatorsById.Values)
            {
                ConsoleWriteAt($"#{elevator.Id}|\tFloor: {elevator.CurrentFloor}\tCapacity:{elevator.Capacity}%", elevator.Id, 0);
            }
        }

        private void ConsoleWriteAt(string message, int row, int column, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.SetCursorPosition(column, row);
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.Write(message);

            Console.ResetColor();
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Dispose()
        {
            Task.Run(async () =>
            {
                await _hubConnection.DisposeAsync();
            }).Wait();
        }
    }
}