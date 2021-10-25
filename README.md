# Elevator App
Elevator App is an event-driven application that simulates a basic elevator. This project is composed of four parts: the `Core` class library, the ASP.NET Core `Server`, the React.js `Client`, and the `Test` xUnit unit test project.

## Usage
First, navigate to the `ElevatorApp.Server` directory and run the command.
```
dotnet run
```
Some settings are customizable by updating the "Building" portion of `appsettings.json`.
**NOTE**: [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) SDK and runtime must be installed on your machine to run the server.
During runtime, all server logs are written to `log.txt` in the `ElevatorApp.Server` folder.


Next, navigate to the `ElevatorApp.Client/client-app` directory and run the following commands.
```
npm install
npm start
```
**NOTE**: [Node.js](https://nodejs.org/en/) must be installed on your machine to run the client.

Finally, open your web browser (i.e., Chrome, Firfox) to [http://localhost:3000](http://localhost:3000). Each browser tab/window opened will be placed in the same "building" on the server, and all elevator requests can be made asynchronously from all open tabs/windows.

## User Interface Explained
### **Elevators**
This table contains real-time data for the elevators in the building.
### **You**
This table contains real-time data of your position and status in the building/elevators.
### **Make a selection**
This section contains button panels. If you are outside the elevator, you can request an elevator by pressing the "up" or "down" buttons. The panel will disappear momentarily while you wait for the next elevator to arrive. Once inside the elevator, each button will represent each floor within the building. Highlighted buttons are active requests.