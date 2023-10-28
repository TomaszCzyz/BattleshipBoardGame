import React from 'react';
import GridBoard from "./GridBoard";

export default PlayerBoards;

function PlayerBoards({playerName}: { playerName: string }) {
  return (
    <div className="PlayerBoards-Content">
      <h3 className="PlayerBoards-header">Player {playerName}</h3>
      <div>
        <p className="PlayerBoards-title">ships board</p>
        <GridBoard/>
      </div>
      <div>
        <p className="PlayerBoards-title">guessing board</p>
        <GridBoard/>
      </div>
    </div>
  );
}
