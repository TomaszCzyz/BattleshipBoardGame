using BattleshipBoardGame.Extensions;
using BattleshipBoardGame.Models;
using JetBrains.Annotations;

namespace BattleshipBoardGame.Services;

[UsedImplicitly]
public class GuessingEngine : IGuessingEngine
{
    public (uint X, uint Y) Guess(Player player)
        => player.GuessingStrategy switch
        {
            GuessingStrategy.Random => GuessRandomly(player),
            _ => throw new ArgumentOutOfRangeException(nameof(player), player, "Unknown guessing strategy")
        };

    private static (uint X, uint Y) GuessRandomly(Player player)
    {
        EnsureCanGuess(player.GuessingBoard);
        uint x, y;
        do
        {
            // todo: replace 'magick' number '10'
            x = (uint)Random.Shared.Next(10);
            y = (uint)Random.Shared.Next(10);
        } while (player.GuessingBoard[x, y] == -1 && !player.Guesses.Contains((x, y)));

        return (x, y);
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
