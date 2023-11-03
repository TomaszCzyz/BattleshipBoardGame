using System.Text.Json.Serialization;
using BattleshipBoardGame.Services;

namespace BattleshipBoardGame.Models.Entities;

/// <summary>
///     Basic information about player
/// </summary>
public class PlayerInfo
{
    public required string Name { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required GuessingStrategy GuessingStrategy { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ShipsPlacementStrategy ShipsPlacementStrategy { get; init; } = ShipsPlacementStrategy.Simple;
}
