export type Simulation = {
  id: string;
  player1: Player;
  player2: Player;
  Winner: Player;
};

export type PlayerInfo = {
  name: string,
  guessingStrategy: string;
  shipsPlacementStrategy: string;
}

export interface Point {
  row: number;
  col: number;
}

type ShipSegment = {
  isSunk: boolean;
  coords: Point
}

type Ship = {
  segments: ShipSegment[]
}

type Player = {
  id: string;
  playerInfo: PlayerInfo;
  guesses: Point[];
  ships: Ship[];
};
