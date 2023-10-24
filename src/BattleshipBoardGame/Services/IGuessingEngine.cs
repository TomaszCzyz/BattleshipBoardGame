using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public interface IGuessingEngine
{
    (int X, int Y) Guess(sbyte[,] guessingBoard, GuessingStrategy guessingStrategy);
}
