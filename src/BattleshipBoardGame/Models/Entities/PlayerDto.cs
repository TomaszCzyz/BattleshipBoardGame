namespace BattleshipBoardGame.Models.Entities;

/// <summary>
///     A model that stores information about player
///     and minimal information, that can be used to
///     reconstruct simulation.
/// </summary>
public class PlayerDto
{
    public required Guid Id { get; init; }

    public required PlayerInfo PlayerInfo { get; init; }

    public required IList<Ship> Ships { get; init; }

    public required IList<(int, int)> Guesses { get; init; }
}
