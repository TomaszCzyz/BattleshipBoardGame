namespace BattleshipBoardGame.Models.Entities;

/// <summary>
///     Representation of a ship
/// </summary>
/// <param name="Type">type of a ship</param>
/// <param name="Segments">list of ship segments</param>
public record Ship(ShipType Type, IList<ShipSegment> Segments);
