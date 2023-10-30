using BattleshipBoardGame.Extensions;
using BattleshipBoardGame.Models.Entities;
using JetBrains.Annotations;

namespace BattleshipBoardGame.Services;

[UsedImplicitly]
public class GuessingEngine : IGuessingEngine
{
    /// <summary>
    ///     Makes a guess based on provided strategy.
    /// </summary>
    /// <returns>
    ///     Coordinates within a <paramref name="guessingBoard"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     No implementation for given <paramref name="guessingStrategy" />
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     No valid guess can be make,
    ///     because there is no unknown point on the <paramref name="guessingBoard"/>,
    ///     <see cref="EnsureCanGuess"/>.
    /// </exception>
    public Point Guess(sbyte[,] guessingBoard, GuessingStrategy guessingStrategy)
    {
        EnsureCanGuess(guessingBoard);

        return GuessInner(guessingBoard, guessingStrategy);
    }

    private static Point GuessInner(sbyte[,] guessingBoard, GuessingStrategy guessingStrategy)
        => guessingStrategy switch
        {
            GuessingStrategy.Random => GuessRandomly(guessingBoard),
            _ => throw new ArgumentOutOfRangeException(nameof(guessingStrategy), guessingStrategy, "Unknown guessing strategy")
        };

    private static Point GuessRandomly(sbyte[,] guessingBoard)
    {
        int x, y;
        do
        {
            x = Random.Shared.Next(guessingBoard.GetLength(0));
            y = Random.Shared.Next(guessingBoard.GetLength(1));
        } while (guessingBoard[x, y] != -1);

        return new Point(x, y);
    }

    /// <summary>
    ///     Check if there is a tile that was not guessed yet.
    /// </summary>
    /// <param name="board">The guessing board of the player</param>
    /// <exception cref="ArgumentException">Throws when there is no tile marked with -1</exception>
    private static void EnsureCanGuess(sbyte[,] board)
    {
        if (board.Enumerate().Any(b => b == -1))
        {
            return;
        }

        throw new ArgumentException("All tiles on the guessing board were already guessed", nameof(board));
    }
}
