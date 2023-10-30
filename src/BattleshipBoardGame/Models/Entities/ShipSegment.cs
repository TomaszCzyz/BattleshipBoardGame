using System.Diagnostics;

namespace BattleshipBoardGame.Models.Entities;

[DebuggerDisplay("{Coords}")]
public class ShipSegment
{
    public Point Coords { get; init; }

    public bool IsSunk { get; set; }
}
