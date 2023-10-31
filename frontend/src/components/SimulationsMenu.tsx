import React, {useEffect, useState} from "react";

export default SimulationsMenu;

interface SimulationsMenuProps {
  onSimLoadClick: (id: string) => void;
}

function SimulationsMenu({simulationsMenuProps}: { simulationsMenuProps: SimulationsMenuProps }) {
  const [simulationNames, setSimulationNames] = useState<string[]>([]);
  let selectedSimIndex = 0;

  useEffect(() => {
    fetch('http://localhost:5000/simulations/battleship/')
      .then((response) => response.json())
      .then((data) => setSimulationNames(data))
      .catch((error) => console.error(error));
  }, []);

  const handleClick = (_: React.MouseEvent<HTMLButtonElement>) => {
    if (selectedSimIndex >= simulationNames.length) {
      return new Error("no valid sim selected");
    }
    simulationsMenuProps.onSimLoadClick(simulationNames[selectedSimIndex]);
  }

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
          <button className="SimulationsMenu-button" onClick={handleClick}>Load</button>
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
