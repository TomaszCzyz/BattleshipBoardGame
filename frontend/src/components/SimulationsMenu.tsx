import React from "react";

export default SimulationsMenu;


function SimulationsMenu() {
  const simulations: string[] = ["sim#1", "sim#2", "sim_with_custom_name"];

  return (
    <div className="SimulationsMenu" >
      <fieldset>
        <legend>Menu</legend>
        <fieldset className="SimulationsMenu-inner-fieldset" style={{display: "flex"}}>
          <legend>Select existing</legend>
          <select placeholder="select simulation">
            {simulations.map((sim) => <option key={sim} value={sim}>{sim}</option>)}
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
      <text className="medium-text">Ships placement strategy:</text>
      <br/>
      <select>
        <option value="random">random</option>
      </select>
      <br/>
      <text className="medium-text">Guessing strategy:</text>
      <br/>
      <select>
        <option value="random">random</option>
      </select>
    </fieldset>
  )
}
