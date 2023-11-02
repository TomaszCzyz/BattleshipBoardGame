import React from "react";
import {Tile} from "../models/sharedModels";

export default GridSquare;

function GridSquare({tile}: { tile: Tile }) {
  const classes = `GridSquare ${tile}-tile`;
  return <div className={classes}></div>;
}
