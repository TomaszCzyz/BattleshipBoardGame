namespace BattleshipBoardGame.Models;

public record Ship(ShipType Type, IList<ShipSegment> Segments);
