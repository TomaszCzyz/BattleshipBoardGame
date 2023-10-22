namespace BattleshipBoardGame;

public class Constants
{
    public static readonly (int I, int J)[] NeighborTilesRelativeCoords
        = { (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1) };

    public const int BoardLength = 10;
}
