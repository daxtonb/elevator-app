import logo from './logo.svg';
import './App.css';
import Elevators from "./components/Elevators";
import Occupant from './components/Occupant';
import ButtonPanel from "./components/ButtonPanel/ButtonPanel";
import { Provider as ReduxProvider } from 'react-redux';
import store from './redux/store';
import { connection } from './utils/elevatorHub';
import { useEffect, useState } from 'react';

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
          <Elevators />
          <Occupant />
          <ButtonPanel />
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
