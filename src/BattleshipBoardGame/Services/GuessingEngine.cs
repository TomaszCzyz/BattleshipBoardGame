using System.Diagnostics;
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
            GuessingStrategy.FromCenter => GuessFromCenter(guessingBoard),
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

    private static Point GuessFromCenter(sbyte[,] guessingBoard)
    {
        foreach (var (row, col) in GetSpiralIndices(guessingBoard))
        {
            if (guessingBoard[row, col] == -1)
            {
                return new Point(row, col);
            }
        }

        throw new UnreachableException("The broad has to contain at least one unknown tile.");
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

    private static IEnumerable<Point> GetSpiralIndices(sbyte[,] array)
    {
        var rows = array.GetLength(0);
        var cols = array.GetLength(1);
        if (rows != cols || rows % 2 != 0)
        {
            throw new ArgumentException("Array dimensions must be equal and even.");
        }

        // up, left, down, right
        var direction = new[] { new Point(-1, 0), new Point(0, -1), new Point(1, 0), new Point(0, 1) };

        var count = array.Length;
        var row = rows >> 1;
        var col = cols >> 1;

        var steps = 1;
        var dirIndex = 0;

        while (count > 0)
        {
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < steps; j++)
                {
                    yield return new Point(row, col);

                    row += direction[dirIndex % 4].Row;
                    col += direction[dirIndex % 4].Col;

                    count--;
                }

                dirIndex++;
            }

            steps++;
        }
    }
}
