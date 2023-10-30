using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame;

public class Constants
{
    public static readonly Point[] NeighborTilesRelativeCoords
        = { new(-1, 0), new(-1, 1), new(0, 1), new(1, 1), new(1, 0), new(1, -1), new(0, -1), new(-1, -1) };

    public const int BoardLength = 10;

    public const string AllowedSpecificOrigins = "AllowedSpecificOrigins";
}
