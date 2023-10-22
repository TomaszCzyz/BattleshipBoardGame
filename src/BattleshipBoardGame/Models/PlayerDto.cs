namespace BattleshipBoardGame.Models;

public class PlayerDto
{
    public required Guid Id { get; init; }

    public required PlayerInfo PlayerInfo { get; init; }

    public required IList<Ship> Ships { get; init; }

    public required IList<(uint, uint)> Guesses { get; init; }
}
