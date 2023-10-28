import React from "react";
import {Tile} from "../models/tile";

export default GridSquare;

function GridSquare({tile}: { tile: Tile }) {
  const classes = `grid-square ${tile}-tile grid-row: 0`;
  return <div className={classes}></div>;
}
