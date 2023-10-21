using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public interface IGuessingEngine
{
    (int X, int Y) Guess(Player player);
}
