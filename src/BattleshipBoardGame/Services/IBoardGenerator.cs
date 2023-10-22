using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public interface IBoardGenerator
{
    public sbyte[,] Generate(ShipsPlacementStrategy strategy = ShipsPlacementStrategy.Simple);

    public IList<Ship> GenerateShips(ShipsPlacementStrategy strategy = ShipsPlacementStrategy.Simple);
}
