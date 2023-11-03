using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame.Services;

public interface IBattleshipGameSimulator
{
    Task Run(Simulation simulation, PlayerInfo playerInfo1, PlayerInfo playerInfo2, CancellationToken cancellationToken);
}
