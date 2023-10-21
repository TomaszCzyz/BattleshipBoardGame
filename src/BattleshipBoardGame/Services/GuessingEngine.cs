using BattleshipBoardGame.Models;
using JetBrains.Annotations;

namespace BattleshipBoardGame.Services;

[UsedImplicitly]
public class GuessingEngine : IGuessingEngine
{
    public (int X, int Y) Guess(Player player)
        => player.GuessingStrategy switch
        {
            GuessingStrategy.Random => GuessRandomly(player),
            _ => throw new ArgumentOutOfRangeException(nameof(player), player, "Unknown guessing strategy")
        };

    private static (int X, int Y) GuessRandomly(Player player)
    {
        int x, y;
        do
        {
            // todo: replace 'magick' number '10'
            x = Random.Shared.Next(10);
            y = Random.Shared.Next(10);
        } while (player.GuessingBoard[x, y] == -1);

        return (x, y);
    }
}
