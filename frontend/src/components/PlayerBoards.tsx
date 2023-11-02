import React from 'react';
import GridBoard from "./GridBoard";
import GridSquare from "./GridSquare";
import {Tile} from "../models/sharedModels";

export default PlayerBoards;

interface PlayerBoardsProps {
  /** The text to display as a player name */
  playerName: string;
  /** The board with player's ships */
  ownBoard: Tile[][];
  /** The board with player's guesses */
  guessingBoard: Tile[][];
}

function PlayerBoards({playerName, ownBoard, guessingBoard}: PlayerBoardsProps) {
  const ownGrid: React.JSX.Element[][] = []
  for (let row = 0; row < 10; row++) {
    ownGrid.push([])
    for (let col = 0; col < 10; col++) {
      ownGrid[row].push(<GridSquare key={`${row}${col}`} tile={ownBoard[row][col]}/>)
    }
  }

  const guessingGrid: React.JSX.Element[][] = []
  for (let row = 0; row < 10; row++) {
    guessingGrid.push([])
    for (let col = 0; col < 10; col++) {
      guessingGrid[row].push(<GridSquare key={`${row}${col}`} tile={guessingBoard[row][col]}/>)
    }
  }

  return (
    <div className="PlayerBoards-Content">
      <h3 className="PlayerBoards-header">Player {playerName}</h3>
      <div>
        <p className="PlayerBoards-title">ships board</p>
        <GridBoard board={ownGrid}/>
      </div>
      <div>
        <p className="PlayerBoards-title">guessing board</p>
        <GridBoard board={guessingGrid}/>
      </div>
    </div>
  );
}
