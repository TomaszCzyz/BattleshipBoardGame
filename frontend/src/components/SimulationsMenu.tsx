import React, {useEffect, useState} from "react";

export default SimulationsMenu;


function SimulationsMenu() {
  const [simulationNames, setSimulationNames] = useState<string[]>(["sim_placeholder"]);

  useEffect(() => {
    fetch('http://localhost:5000/simulations/battleship/')
      .then((response) => response.json())
      .then((data) => setSimulationNames(data))
      .catch((error) => console.error(error));
  }, []);

  return (
    <div className="SimulationsMenu">
      <fieldset>
        <legend>Menu</legend>
        <fieldset className="SimulationsMenu-inner-fieldset">
          <legend>Select existing</legend>
          <select placeholder="select simulation" defaultValue="placeholder">
            <option value="placeholder" disabled>Select sim</option>
            {simulationNames.map((sim) => <option key={sim} value={sim}>{sim}</option>)}
          </select>
          <button className="SimulationsMenu-button">Load</button>
        </fieldset>
        <fieldset className="SimulationsMenu-inner-fieldset">
          <legend>Run new</legend>
          <div style={{display: "flex", flexWrap: "wrap"}}>
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
