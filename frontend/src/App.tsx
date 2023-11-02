import React, {useState} from 'react';
import './App.css';
import PlayerBoards from "./components/PlayerBoards";
import {Tile} from "./models/tile";
import SimulationsMenu from "./components/SimulationsMenu";
import {Simulation} from "./models/sharedModels";
import NextGuessImage from './assets/icons8-forward-100.png';
import PrevGuessImage from './assets/icons8-back-100.png';
import DoubleNextGuessImage from './assets/icons8-double-right-100.png';
import DoublePrevGuessImage from './assets/icons8-double-left-100.png';
import FastForwardImage from './assets/icons8-fast-forward-100.png';

export default App

function App() {
  // let ownBoardA: Tile[][] = initialize2DArray(10, "sea");
  const initialHist: string[] = [];
  const [history, setHistory] = useState(initialHist)

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

        setHistory(createSimHistory(sim))
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
        <div className="App-sim-history medium-text">
          <h4>Simulation history:</h4>
          <div className="App-sim-hist">
            <button><img src={DoublePrevGuessImage} alt="backward 10 guesses"/></button>
            <button><img src={PrevGuessImage} alt="previous guess"/></button>
            <button><img src={NextGuessImage} alt="next guess"/></button>
            <button><img src={DoubleNextGuessImage} alt="forward 10 guesses"/></button>
            <button><img src={FastForwardImage} alt="forward to the end"/></button>
          </div>
          <ol style={{height: "50px"}}>
            {
              history.map((guess, i) =>
                <li key={i}>
                  <button>{guess}</button>
                </li>)
            }
          </ol>
        </div>
      </div>
    </div>
  );
}

const createSimHistory = (sim: Simulation): string[] => {
  let guesses1 = sim.player1.guesses.map(guess => `P1 guessed: (${guess.row},${guess.col})`);
  let guesses2 = sim.player2.guesses.map(guess => `P2 guessed: (${guess.row},${guess.col})`);
  return guesses1
    .map((e, i) => [e, guesses2[i]])
    .flatMap(guessesInRound => guessesInRound)
}

const initialize2DArray = (size: number, val: Tile = "unknown"): Tile[][] =>
  Array.from({length: size}, () => Array(size).fill(val));
