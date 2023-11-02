import React, {useEffect, useState} from "react";
import {PlayerInfo} from "../models/sharedModels";

export default SimulationsMenu;

interface SimulationsMenuProps {
  onSimLoadClick: (id: string) => void;
  onSimRunClick: (player1: PlayerInfo, player2: PlayerInfo) => void;
}

function SimulationsMenu({simulationsMenuProps}: { simulationsMenuProps: SimulationsMenuProps }) {
  const [simulationNames, setSimulationNames] = useState<string[]>([]);
  // marker to trigger simulation list reloading when new simulation has been run
  const [newSimMarker, setNewSimMarker] = useState(true);
  let playerInfo1: PlayerInfo = {
    name: "1",
    guessingStrategy: "random",
    shipsPlacementStrategy: "simple",
  };
  let playerInfo2: PlayerInfo = {
    name: "2",
    guessingStrategy: "random",
    shipsPlacementStrategy: "simple",
  };
  let selectedSimIndex = 0;

  useEffect(() => {
    fetch('http://localhost:5000/simulations/battleship/')
      .then(response => response.json())
      .then(data => setSimulationNames(data))
      .catch(error => console.error(error));
  }, [newSimMarker]);

  const handleLoadSimClick = (_: React.MouseEvent<HTMLButtonElement>) => {
    if (selectedSimIndex >= simulationNames.length) {
      return new Error("no valid sim selected");
    }
    simulationsMenuProps.onSimLoadClick(simulationNames[selectedSimIndex]);
  }

  const handleRunSimClick = () => {
    setNewSimMarker(!newSimMarker);
    simulationsMenuProps.onSimRunClick(playerInfo1, playerInfo2)
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
          <button className="SimulationsMenu-button" onClick={handleLoadSimClick}>Load</button>
        </fieldset>
        <fieldset className="SimulationsMenu-inner-fieldset">
          <legend>Run new</legend>
          <div className="SimulationsMenu-player-settings">
            <PlayerSettings playerInfo={playerInfo1} updatePlayerInfo={playerInfo => playerInfo1 = playerInfo}/>
            <PlayerSettings playerInfo={playerInfo2} updatePlayerInfo={playerInfo => playerInfo2 = playerInfo}/>
          </div>
          <button className="SimulationsMenu-button" onClick={handleRunSimClick}>Run</button>
        </fieldset>
      </fieldset>
    </div>
  );
}

function PlayerSettings({playerInfo, updatePlayerInfo}: { playerInfo: PlayerInfo, updatePlayerInfo: (playerInfo: PlayerInfo) => void }) {
  return (
    <fieldset style={{border: "1px solid"}}>
      <legend>Player {playerInfo.name}</legend>
      <span className="medium-text">Ships placement strategy:</span>
      <br/>
      <select onChange={e => updatePlayerInfo({
        ...playerInfo,
        shipsPlacementStrategy: e.target.value
      })}>
        <option value="simple">simple</option>
      </select>
      <br/>
      <span className="medium-text">Guessing strategy:</span>
      <br/>
      <select onChange={e => updatePlayerInfo({
        ...playerInfo,
        guessingStrategy: e.target.value
      })}>
        <option value="random">random</option>
        <option value="advanced">advanced</option>
      </select>
    </fieldset>
  )
}
