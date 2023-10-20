namespace BattleshipBoardGame.Models;

public class Player
{
    public required sbyte[][] Board { get; set; }

    public required string[] Guesses { get; set; }
}
