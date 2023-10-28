import React from "react";

export default GridBoard;

function GridBoard({board}: { board: React.JSX.Element[][] }) {
  return (
    <div className='GridBoard'>
      {board}
    </div>
  )
}
