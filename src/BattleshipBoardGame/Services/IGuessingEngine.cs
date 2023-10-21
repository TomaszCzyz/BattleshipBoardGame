using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public interface IGuessingEngine
{
    (uint X, uint Y) Guess(Player player);
}
