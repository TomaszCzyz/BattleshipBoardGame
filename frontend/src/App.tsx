import React, {useState} from 'react';
import './App.css';
import PlayerBoards from "./components/PlayerBoards";
import {Tile} from "./models/tile";
import SimulationsMenu from "./components/SimulationsMenu";
import {Simulation} from "./models/sharedModels";
import retryTimes = jest.retryTimes;

export default App

function App() {
  // let ownBoardA: Tile[][] = initialize2DArray(10, "sea");
  let guessingBoardA: Tile[][] = initialize2DArray(10, "sea");
  let ownBoardB: Tile[][] = initialize2DArray(10, "sea");
  let guessingBoardB: Tile[][] = initialize2DArray(10, "sea");

  const [ownBoardA, setOwnBoardA] = useState(initialize2DArray(10, "sea"));

  const handleLoadSim = (simId: string) => {
    fetch(`http://localhost:5000/simulations/battleship/${simId}`)
      .then(response => response.json() as Promise<Simulation>)
      .then(sim => {
        const shipsCoords = sim.player1.ships
          .flatMap(ship => ship.segments)
          .map(seg => seg.coords);

        let array = initialize2DArray(10, "sea");
        array.forEach((rowTiles, row) => {
          rowTiles.forEach((_, col) => {
            if (shipsCoords.some(coord => coord.row === row && coord.col === col)) {
              array[row][col] = "ship";
            }
          })
        })

        setOwnBoardA(array);
      })
      .catch(error => console.error(error));
  };

  return (
    <div className="App">
      <header className="App-header">
        Battleship board game simulator
      </header>
      <SimulationsMenu simulationsMenuProps={{onSimLoadClick: handleLoadSim}}/>
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
