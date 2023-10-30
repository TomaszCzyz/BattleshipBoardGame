import React, {useEffect, useState} from "react";

export default SimulationsMenu;

type Simulation = {
  id: number;
  Player1: Player;
  Player2: Player;
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

function SimulationsMenu() {
  const [simulationNames, setSimulationNames] = useState<string[]>([]);
  let selectedSimIndex = 0;

  useEffect(() => {
    fetch('http://localhost:5000/simulations/battleship/')
      .then((response) => response.json())
      .then((data) => setSimulationNames(data))
      .catch((error) => console.error(error));
  }, []);

  const handleLoadSim = () => {
    if (selectedSimIndex < 0) {
      console.error(new Error("Simulation index is not valid"));
      return;
    }
    const id = simulationNames[selectedSimIndex];
    fetch(`http://localhost:5000/simulations/battleship/${id}`)
      .then(response => response.json() as Promise<Simulation>)
      .catch(error => console.error(error));
  };

  return (
    <div className="SimulationsMenu">
      <fieldset>
        <legend>Menu</legend>
        <fieldset className="SimulationsMenu-inner-fieldset">
          <legend>Select existing</legend>
          <select placeholder="select simulation" defaultValue="placeholder" onChange={e => selectedSimIndex = e.target.selectedIndex - 1}>
            <option value="placeholder" disabled>Select sim</option>
            {simulationNames.map((sim) => <option key={sim} value={sim}>{sim}</option>)}
          </select>
          <button className="SimulationsMenu-button" onClick={handleLoadSim}>Load</button>
        </fieldset>
        <fieldset className="SimulationsMenu-inner-fieldset">
          <legend>Run new</legend>
          <div className="SimulationsMenu-player-settings">
            <PlayerSettings name={"1"}/>
            <PlayerSettings name={"2"}/>
          </div>
          <button className="SimulationsMenu-button">Run</button>
        </fieldset>
      </fieldset>
    </div>
  );
}

function PlayerSettings({name}: { name: string }) {
  return (
    <fieldset style={{border: "1px solid"}}>
      <legend>Player {name}</legend>
      <span className="medium-text">Ships placement strategy:</span>
      <br/>
      <select>
        <option value="random">random</option>
      </select>
      <br/>
      <span className="medium-text">Guessing strategy:</span>
      <br/>
      <select>
        <option value="random">random</option>
      </select>
    </fieldset>
  )
}
