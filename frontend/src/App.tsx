import React from 'react';
import './App.css';
import PlayerBoards from "./components/PlayerBoards";

export default App

function App() {
  return (
    <div className="App">
      <header className="App-header">
        Battleship board game simulator
      </header>
      <div className="App-Content">
        <div className="App-Boards">
          <PlayerBoards playerName={"A"}/>
          <PlayerBoards playerName={"B"}/>
        </div>
        <div className="sim-history">
        </div>
      </div>
    </div>
  );
}

