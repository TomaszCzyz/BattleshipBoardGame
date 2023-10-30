using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame.Services;

public interface IGuessingEngine
{
    Point Guess(sbyte[,] guessingBoard, GuessingStrategy guessingStrategy);
}
