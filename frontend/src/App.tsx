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
      <div className="App-Content">
        <div className="boards-grid">
          <h3 className="player-header">Player 1</h3>
          <div>
            <p className="board-title">ships board</p>
            <GridBoard/>
          </div>
          <div>
            <p className="board-title">guessing board</p>
            <GridBoard/>
          </div>
          <h3 className="player-header">Player 2</h3>
          <div>
            <p className="board-title">ships board</p>
            <GridBoard/>
          </div>
          <div>
            <p className="board-title">guessing board</p>
            <GridBoard/>
          </div>
        </div>
        <div className="sim-history">
        </div>
      </div>
    </div>
  );
}

