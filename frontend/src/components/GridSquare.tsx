import React from "react";

export default GridSquare;

type Tile = "sea" | "ship"

function GridSquare({tile}: { tile: Tile }) {
  const classes = `grid-square ${tile}-tile grid-row: 0`;
  return <div className={classes}></div>;
}
