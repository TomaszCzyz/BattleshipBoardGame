using BattleshipBoardGame.Models;
using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame.Services;

public interface IBoardGenerator
{
    public IList<Ship> GenerateShips(ShipsPlacementStrategy strategy = ShipsPlacementStrategy.Simple);
}
