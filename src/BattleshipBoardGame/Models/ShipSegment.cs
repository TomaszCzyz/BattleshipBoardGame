namespace BattleshipBoardGame.Models;

public class ShipSegment
{
    public (uint X, uint Y) Coords { get; init; }

    public bool IsSunk { get; set; }
}
