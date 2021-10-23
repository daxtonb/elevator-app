using System;
using System.Diagnostics;

namespace ElevatorApp.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port = GetIntFromuser("Enter server port");

                using (var app = new App(port))
                {
                    Process.GetCurrentProcess().WaitForExit();
                }
            }
            catch (System.Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error ocurred: {ex.Message}");
            }
        }



        /// <summary>
        /// Fetches and validates user input for an integer value
        /// </summary>
        static int GetIntFromuser(string prompt)
        {
            while (true)
            {
                Console.ResetColor();
                Console.Write($"{prompt}: ");

                if (int.TryParse(Console.ReadLine(), out int input))
                {
                    return input;
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
