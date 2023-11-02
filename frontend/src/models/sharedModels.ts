export type Simulation = {
  id: number;
  player1: Player;
  player2: Player;
  Winner: Player;
};

type PlayerInfo = {
  guessingStrategy: string;
  shipsPlacementStrategy: string;
}

interface Point {
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
