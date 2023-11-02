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
  const emptyBoard = initialize2DArray(10, "sea")
  const initialBoards: Boards = {ownA: emptyBoard, ownB: emptyBoard, guessingA: emptyBoard, guessingB: emptyBoard};

  const [simulation, setSimulation] = useState<Simulation | null>(null);
  const [boards, setBoards] = useState<Boards>(initialBoards);
  const [history, setHistory] = useState<string[]>([])

  const loadRound = (roundNumber: number) => {
    if (simulation === null || roundNumber < 0 || roundNumber >= history.length) {
      return;
    }
    const newBoards = getBoards(roundNumber, simulation);
    setBoards(newBoards);
  }

  const handleLoadSim = (simId: string) => {
    fetch(`http://localhost:5000/simulations/battleship/${simId}`)
      .then(response => response.json() as Promise<Simulation>)
      .then(sim => {
        const newGuesses = createGuesses(sim);
        setHistory(newGuesses)
        setSimulation(sim);
      })
      .then(_ => loadRound(0))
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

const createGuesses = (sim: Simulation): string[] => {
  let guesses1 = sim.player1.guesses.map(guess => `P1 guessed: (${guess.row},${guess.col})`);
  let guesses2 = sim.player2.guesses.map(guess => `P2 guessed: (${guess.row},${guess.col})`);
  return guesses1
    .map((e, i) => [e, guesses2[i]])
    .flatMap(guessesInRound => guessesInRound);
}

const getBoards = (roundNumber: number, sim: Simulation): Boards => {
  const ownA = initialize2DArray(10, "sea");
  const ownB = initialize2DArray(10, "sea");
  const guessingA = initialize2DArray(10, "sea");
  const guessingB = initialize2DArray(10, "sea");

  const shipCoordsA = sim.player1.ships.flatMap(s => s.segments.map(v => v.coords));
  const shipCoordsB = sim.player2.ships.flatMap(s => s.segments.map(v => v.coords));

  const guessesA = sim.player1.guesses.slice(0, roundNumber);
  const guessesB = sim.player2.guesses.slice(0, roundNumber);

  // draw ships
  shipCoordsA.forEach(p => ownA[p.row][p.col] = "ship");
  shipCoordsB.forEach(p => ownB[p.row][p.col] = "ship");

  // draw guesses
  guessesA.forEach(p => guessingA[p.row][p.col] = "miss");
  guessesB.forEach(p => guessingB[p.row][p.col] = "miss");

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
