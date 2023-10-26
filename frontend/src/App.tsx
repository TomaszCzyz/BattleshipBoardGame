import React from 'react';
import './App.css';
import GridBoard from "./components/GridBoard";

export default App

function App() {
  return (
    <div className="App">
      <header className="App-header">
        Battleship board game simulator
      </header>
      <div className="App-content">
        <div>
          <GridBoard/>
        </div>
        <div>
          <GridBoard/>
        </div>
        <div>
          <GridBoard/>
        </div>
        <div>
          <GridBoard/>
        </div>
      </div>
    </div>
  );
}

