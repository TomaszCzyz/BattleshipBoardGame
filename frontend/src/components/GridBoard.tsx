import React from "react";
import GridSquare from "./GridSquare";

export default GridBoard;

function GridBoard() {
  const grid: React.JSX.Element[][] = []
  for (let row = 0; row < 10; row++) {
    grid.push([])
    for (let col = 0; col < 10; col++) {
      grid[row].push(<GridSquare key={`${col}${row}`} tile="sea"/>)
    }
  }
  
  return (
    <div className='GridBoard'>
      {grid}
    </div>
  )
}
