import React, {useState} from 'react';
import './App.css';
import PlayerBoards from "./components/PlayerBoards";
import SimulationsMenu from "./components/SimulationsMenu";
import {PlayerInfo, Simulation, Tile} from "./models/sharedModels";
import NextGuessImage from './assets/icons8-forward-100.png';
import PrevGuessImage from './assets/icons8-back-100.png';
import DoubleNextGuessImage from './assets/icons8-double-right-100.png';
import DoublePrevGuessImage from './assets/icons8-double-left-100.png';
import FastForwardImage from './assets/icons8-fast-forward-100.png';

export default App

type Boards = {
  ownA: Tile[][],
  ownB: Tile[][],
  guessingA: Tile[][],
  guessingB: Tile[][],
}

function App() {
  const emptySeaBoard = initialize2DArray(10, "sea")
  const emptyUnknownBoard = initialize2DArray(10, "unknown")
  const initialBoards: Boards = {ownA: emptySeaBoard, ownB: emptySeaBoard, guessingA: emptyUnknownBoard, guessingB: emptyUnknownBoard};

  const [simulation, setSimulation] = useState<Simulation | null>(null);
  const [boards, setBoards] = useState<Boards>(initialBoards);

  const loadRound = (roundNumber: number, sim: Simulation) => {
    if (sim === null || roundNumber < 0 || roundNumber > sim.player1.guesses.length) {
      return;
    }
    const newBoards = getBoards(roundNumber, sim);
    setBoards(newBoards);
  }

  const handleLoadSim = (simId: string) => {
    fetch(`http://localhost:5000/simulations/battleship/${simId}`)
      .then(response => response.json() as Promise<Simulation>)
      .then(sim => {
        setSimulation(sim);
        loadRound(0, sim);
      })
      .catch(error => console.error(error));
  };

  const handleRunSim = (playerInfo1: PlayerInfo, playerInfo2: PlayerInfo) => {
    const runNewSim = async () =>
      await fetch(
        `http://localhost:5000/simulations/battleship/`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify([playerInfo1, playerInfo2]),
        })
        .then(sim => sim.json())
        .catch(error => console.error(error));

    runNewSim().then(simId => {
      if (simId) {
        handleLoadSim(simId)
      }
    });
  };

  return (
    <div className="App">
      <header className="App-header">
        Battleship board game simulator
      </header>
      <SimulationsMenu simulationsMenuProps={{onSimLoadClick: handleLoadSim, onSimRunClick: handleRunSim}}/>
      <div className="App-Content">
        <div className="App-Boards">
          <PlayerBoards playerName={"A"} ownBoard={boards.ownA} guessingBoard={boards.guessingA}/>
          <PlayerBoards playerName={"B"} ownBoard={boards.ownB} guessingBoard={boards.guessingB}/>
        </div>
        <div className="App-sim-history medium-text">
          <h4>Simulation {simulation?.id.substring(0, 4)}(...) history:</h4>
          <div className="App-sim-hist">
            <button><img src={DoublePrevGuessImage} alt="backward 10 guesses"/></button>
            <button><img src={PrevGuessImage} alt="previous guess"/></button>
            <button><img src={NextGuessImage} alt="next guess"/></button>
            <button><img src={DoubleNextGuessImage} alt="forward 10 guesses"/></button>
            <button><img src={FastForwardImage} alt="forward to the end"/></button>
          </div>
          <ol style={{height: "50px"}}>
            {
              simulation?.player1.guesses
                .map((guess1, i) => [guess1, simulation?.player2.guesses[i]])
                .map(([g1, g2], i) =>
                  <li key={i}>
                    <button onClick={_ => loadRound(i + 1, simulation)}>
                      P1 guessed: ({g1.row},{g1.col})
                      <br/>
                      P2 guessed: ({g2.row},{g2.col})
                    </button>
                  </li>
                )
            }
          </ol>
        </div>
      </div>
    </div>
  );
}

const getBoards = (roundNumber: number, sim: Simulation): Boards => {
  const ownA = initialize2DArray(10, "sea");
  const ownB = initialize2DArray(10, "sea");
  const guessingA = initialize2DArray(10, "unknown");
  const guessingB = initialize2DArray(10, "unknown");

  const shipCoordsA = sim.player1.ships.flatMap(s => s.segments.map(v => v.coords));
  const shipCoordsB = sim.player2.ships.flatMap(s => s.segments.map(v => v.coords));

  const guessesA = sim.player1.guesses.slice(0, roundNumber);
  const guessesB = sim.player2.guesses.slice(0, roundNumber);

  // draw ships
  shipCoordsA.forEach(p => ownA[p.row][p.col] = "ship");
  shipCoordsB.forEach(p => ownB[p.row][p.col] = "ship");

  // draw guesses
  guessesA.forEach(p => guessingA[p.row][p.col] = "hit-sea");
  guessesB.forEach(p => guessingB[p.row][p.col] = "hit-sea");

  // mark hit ships
  guessesA
    .filter(p => shipCoordsB.some(value => value.row === p.row && value.col === p.col))
    .forEach(p => {
      guessingA[p.row][p.col] = "hit";
      ownB[p.row][p.col] = "hit";
    });

  guessesB
    .filter(p => shipCoordsA.some(value => value.row === p.row && value.col === p.col))
    .forEach(p => {
      guessingB[p.row][p.col] = "hit";
      ownA[p.row][p.col] = "hit";
    });

  return {ownA, ownB, guessingA, guessingB};
}

const initialize2DArray = (size: number, val: Tile = "unknown"): Tile[][] =>
  Array.from({length: size}, () => Array(size).fill(val));
