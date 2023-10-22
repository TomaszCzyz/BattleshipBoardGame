using System.Diagnostics;

namespace BattleshipBoardGame.Models;

[DebuggerDisplay("{Coords}")]
public class ShipSegment
{
    public (int X, int Y) Coords { get; init; }

    public bool IsSunk { get; set; }
}
