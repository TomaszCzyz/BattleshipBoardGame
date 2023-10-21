using System.ComponentModel.DataAnnotations.Schema;

namespace BattleshipBoardGame.Models;

public class Player
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public required GuessingStrategy GuessingStrategy { get; init; }

    public required sbyte[,] Board { get; init; }

    public required IList<string> Guesses { get; set; }

    [NotMapped]
    public sbyte[,] GuessingBoard { get; init; } = new sbyte[10, 10];

    [NotMapped]
    public uint HitCounter { get; set; }

    /// <summary>
    ///     Answers to another player's guess.
    /// </summary>
    /// <param name="guess">Coordinates on the board</param>
    /// <returns>
    ///     True - if a ship was hit,
    ///     False - when missed.
    /// </returns>
    public bool Answer((int X, int Y) guess)
    {
        return Board[guess.X, guess.Y] == 1;
    }
}
