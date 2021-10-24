import logo from './logo.svg';
import './App.css';
import Elevators from "./components/Elevators";
import { Provider as ReduxProvider } from 'react-redux';
import store from './redux/store';
import { connection } from './utils/elevatorHub';
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

function App() {
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    if (!isConnected) {
      connection.start().then(res => {
        setIsConnected(true);
        console.log('connected');
      }).catch(err => 'connection failed');
    }
  }, [isConnected]);


  return (
    <>
      {isConnected && <ReduxProvider store={store}>
        <div className="App">
          <header className="App-header">
            <img src={logo} className="App-logo" alt="logo" />
            <Elevators />
            <a
              className="App-link"
              href="https://reactjs.org"
              target="_blank"
              rel="noopener noreferrer"
            >
              Learn React
            </a>
          </header>
        </div>
      </ReduxProvider>}
      {
        !isConnected &&
        <div>connecting...</div>
      }
    </>
  );
}

export default App;
