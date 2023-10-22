using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BattleshipBoardGame.Models;

namespace BattleshipBoardGame.Services;

public enum ShipsPlacementStrategy
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

    public sbyte[,] Generate(ShipsPlacementStrategy strategy = ShipsPlacementStrategy.Simple)
        => strategy switch
        {
            ShipsPlacementStrategy.Simple => GenerateSimple(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, $"No implementation for strategy {strategy} yet.")
        };

    public IList<Ship> GenerateShips(ShipsPlacementStrategy strategy = ShipsPlacementStrategy.Simple)
        => strategy switch
        {
            ShipsPlacementStrategy.Simple => GenerateShipsSimple(),
            _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, $"No implementation for strategy {strategy} yet.")
        };

    /// <summary>
    ///     Naive implementation of ships placements calculations.
    ///     For each <see cref="ShipType" /> we randomly choose a point on a board and a direction.
    ///     Then we check if there is no collision with edge or another ship. If there is a collision,
    ///     then we repeat the draw.
    /// </summary>
    /// <returns>A board with ships placement</returns>
    private static List<Ship> GenerateShipsSimple()
    {
        var ships = new List<Ship>();

        foreach (var shipType in Enum.GetValues<ShipType>())
        {
            var qty = 0;
            while (qty != _shipQty[shipType])
            {
                Ship? ship;
                while (!TryGenerateShip(shipType, out ship))
                {
                }

                if (!CanPlaceShip2(ship, ships))
                {
                    continue;
                }

                ships.Add(ship);
                qty++;
            }
        }

        return ships;
    }

    /// <summary>
    ///     Checks if there is a collision with another ship.
    /// </summary>
    private static bool CanPlaceShip2(Ship ship, IEnumerable<Ship> ships)
    {
        var alreadyPlaced = ships.SelectMany(s => s.Segments).Select(segment => segment.Coords).ToArray();

        foreach (var coords in ship.Segments.Select(s => s.Coords))
        {
            // check if there is a ship segment at this coordinates or coordinates adjacent to it.
            var forbiddenCoords = Constants.NeighborTilesRelativeCoords
                .Select(relative => (coords.X + relative.I, coords.Y + relative.J))
                .Append(coords)
                .ToArray();

            if (alreadyPlaced.Any(c => forbiddenCoords.Contains(c)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryGenerateShip(ShipType shipType, [NotNullWhen(true)] out Ship? ship)
    {
        ship = null;
        var x = Random.Shared.Next(BoardLength);
        var y = Random.Shared.Next(BoardLength);
        var dir = (Dir)Random.Shared.Next(4);

        var len = (int)_shipSizes[shipType];
        var relativeCoords = Enumerable.Range(0, len).ToArray();

        var (xCoords, yCoords) = dir switch
        {
            Dir.Up => (relativeCoords.Select(i => x - i), Enumerable.Repeat(y, len)),
            Dir.Right => (Enumerable.Repeat(x, len), relativeCoords.Select(i => y + i)),
            Dir.Down => (relativeCoords.Select(i => x + i), Enumerable.Repeat(y, len)),
            Dir.Left => (Enumerable.Repeat(x, len), relativeCoords.Select(i => y - i)),
            _ => throw new UnreachableException()
        };

        var shipSegments = new List<ShipSegment>();

        foreach (var (i, j) in xCoords.Zip(yCoords))
        {
            // if random points and direction result in ship being outside board bounds, repeat
            if (i < 0 || j < 0 || i >= BoardLength || j >= BoardLength)
            {
                return false;
            }

            shipSegments.Add(new ShipSegment { Coords = (i, j), IsSunk = false });
        }

        ship = new Ship(shipType, shipSegments);
        return true;
    }

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
    ///     There are overlapping checks, however in strategy <see cref="ShipsPlacementStrategy.Simple"/> we go for
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
