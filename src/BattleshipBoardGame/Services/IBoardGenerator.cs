using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public interface IBoardGenerator
{
    public IList<Ship> GenerateShips(ShipsPlacementStrategy strategy = ShipsPlacementStrategy.Simple);
}
