using System.Diagnostics.CodeAnalysis;
using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public enum Strategy
{
    Simple = 0
}

public class BoardGenerator : IBoardGenerator
{
    private const int BoardLength = 10;

    private static readonly Dictionary<ShipType, uint> _shipSizes
        = new()
        {
            { ShipType.Carrier, 5 },
            { ShipType.Battleship, 4 },
            { ShipType.Cruiser, 3 },
            { ShipType.Destroyer, 2 },
            { ShipType.Submarine, 1 }
        };

    private static readonly Dictionary<ShipType, uint> _shipQty
        = new()
        {
            { ShipType.Carrier, 1 },
            { ShipType.Battleship, 1 },
            { ShipType.Cruiser, 1 },
            { ShipType.Destroyer, 2 },
            { ShipType.Submarine, 2 }
        };

    private static readonly (int, int)[] _neighborTiles
        = { (-1, 0), (-1, 1), (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1) };

    public sbyte[,] Generate(Strategy strategy = Strategy.Simple)
        => strategy switch
        {
            Strategy.Simple => GenerateSimple(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, $"No implementation for strategy {strategy} yet.")
        };

    /// <summary>
    ///     Naive implementation of ships placements calculations.
    ///     For each <see cref="ShipType" /> we randomly choose a point on a board and a direction.
    ///     Then we check if there is no collision with edge or another ship. If there is a collision,
    ///     then we repeat the draw.
    /// </summary>
    /// <returns>A board with ships placement</returns>
    private static sbyte[,] GenerateSimple()
    {
        var board = new sbyte[BoardLength, BoardLength];

        foreach (var shipType in Enum.GetValues<ShipType>())
        {
            var qty = 0;
            while (qty != _shipQty[shipType])
            {
                if (!CanPlaceShip(shipType, ref board, out var pointAndDir))
                {
                    continue;
                }

                PlaceShip(shipType, ref board, pointAndDir.Value);
                qty++;
            }
        }

        return board;
    }

    /// <summary>
    ///     To check if ship can be placed we first check, if it fits in board bounds
    ///     and then we check if each tile of the ship would not be adjacent to other ship.
    ///     There are overlapping checks, however in strategy <see cref="Strategy.Simple"/> we go for
    ///     simplicity of implementation.
    /// </summary>
    private static bool CanPlaceShip(ShipType shipType, ref sbyte[,] board, [NotNullWhen(true)] out (int, int, Dir)? pointAndDir)
    {
        pointAndDir = null;
        var shipSize = _shipSizes[shipType];

        var x = Random.Shared.Next(BoardLength);
        var y = Random.Shared.Next(BoardLength);
        var dir = (Dir)Random.Shared.Next(4);

        // check board's edge collision
        if ((dir is Dir.Up && x - shipSize + 1 < 0)
            || (dir is Dir.Down && x + shipSize >= BoardLength)
            || (dir is Dir.Left && y - shipSize + 1 < 0)
            || (dir is Dir.Right && y + shipSize >= BoardLength))
        {
            return false;
        }

        var dirSign = dir is Dir.Down or Dir.Right ? 1 : -1;

        if (dir is Dir.Up or Dir.Down)
        {
            for (var i = x; i != x + (dirSign * shipSize); i += dirSign)
            {
                if (IsAdjacent(ref board, i, y))
                {
                    return false;
                }
            }
        }
        else
        {
            for (var i = y; i != y + (dirSign * shipSize); i += dirSign)
            {
                if (IsAdjacent(ref board, x, i))
                {
                    return false;
                }
            }
        }

        pointAndDir = (x, y, dir);
        return true;
    }

    private static bool IsAdjacent(ref sbyte[,] board, int x, int y)
    {
        foreach (var (i, j) in _neighborTiles)
        {
            if (x + i >= BoardLength || x + i < 0 || y + j >= BoardLength || y + j < 0)
            {
                // tiles beyond the edge
                continue;
            }

            if (board[x + i, y + j] == 1)
            {
                return true;
            }
        }

        return false;
    }

    private static void PlaceShip(ShipType shipType, ref sbyte[,] board, (int, int, Dir) pointAndDir)
    {
        var shipSize = _shipSizes[shipType];
        var (x, y, dir) = pointAndDir;
        switch (dir)
        {
            case Dir.Up:
                for (var i = 0; i < shipSize; i++)
                {
                    board[x - i, y] = 1;
                }

                break;
            case Dir.Down:
                for (var i = 0; i < shipSize; i++)
                {
                    board[x + i, y] = 1;
                }

                break;
            case Dir.Right:
                for (var i = 0; i < shipSize; i++)
                {
                    board[x, y + i] = 1;
                }

                break;
            case Dir.Left:
                for (var i = 0; i < shipSize; i++)
                {
                    board[x, y - i] = 1;
                }

                break;
        }
    }

    private enum Dir
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
}
