using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame.Services;

public enum ShipsPlacementStrategy
{
    Simple = 0
}

/// <summary>
///     A class with logic for generating classic Battleship board
///     containing 5 types of ships, 7 ships total.
/// </summary>
public class BoardGenerator : IBoardGenerator
{
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

    /// <summary>
    ///     Generates a set of <see cref="Ship"/>s using given strategy.
    /// </summary>
    /// <returns>List of ships</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     when strategy is unknown or has no implementation.
    /// </exception>
    /// <seealso cref="GenerateShipsSimple"/>
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
                .Select(relative => new Point(coords.Row + relative.Row, coords.Col + relative.Col))
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
        var x = Random.Shared.Next(Constants.BoardLength);
        var y = Random.Shared.Next(Constants.BoardLength);
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
            if (i < 0 || j < 0 || i >= Constants.BoardLength || j >= Constants.BoardLength)
            {
                return false;
            }

            shipSegments.Add(new ShipSegment { Coords = new Point(i, j), IsSunk = false });
        }

        ship = new Ship(shipType, shipSegments);
        return true;
    }

    private enum Dir
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
}
