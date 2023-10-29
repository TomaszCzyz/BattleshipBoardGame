using BattleshipBoardGame.Services;

namespace BattleshipBoardGame.Models.Entities;

/// <summary>
///     Basic information about player
/// </summary>
public class PlayerInfo
{
    public required string Name { get; init; }

    public required GuessingStrategy GuessingStrategy { get; init; }

    public ShipsPlacementStrategy ShipsPlacementStrategy { get; init; } = ShipsPlacementStrategy.Simple;
}
