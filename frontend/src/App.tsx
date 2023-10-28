import React from 'react';
import './App.css';
import PlayerBoards from "./components/PlayerBoards";
import {Tile} from "./models/tile";
import SimulationsMenu from "./components/SimulationsMenu";

export default App

function App() {
  let ownBoardA: Tile[][] = initialize2DArray(10, "sea");
  let guessingBoardA: Tile[][] = initialize2DArray(10, "sea");
  let ownBoardB: Tile[][] = initialize2DArray(10, "sea");
  let guessingBoardB: Tile[][] = initialize2DArray(10, "sea");

  guessingBoardA[4][4] = "unknown"
  return (
    <div className="App">
      <header className="App-header">
        Battleship board game simulator
      </header>
      <SimulationsMenu/>
      <div className="App-Content">
        <div className="App-Boards">
          <PlayerBoards playerName={"A"} ownBoard={ownBoardA} guessingBoard={guessingBoardA}/>
          <PlayerBoards playerName={"B"} ownBoard={ownBoardB} guessingBoard={guessingBoardB}/>
        </div>
        <div className="sim-history">
        </div>
      </div>
    </div>
  );
}

const initialize2DArray = (size: number, val: Tile = "unknown"): Tile[][] =>
  Array.from({length: size}, () => Array(size).fill(val));
