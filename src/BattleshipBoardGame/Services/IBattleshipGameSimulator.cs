using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame.Services;

public interface IBattleshipGameSimulator
{
    Task Run(Simulation simulation);
}
