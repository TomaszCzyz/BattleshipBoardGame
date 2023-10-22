namespace BattleshipBoardGame.Models;

public class Player
{
    private readonly IList<Ship> _ships;

    public IReadOnlyList<Ship> Ships => _ships.AsReadOnly();

    public GuessingStrategy GuessingStrategy { get; } = GuessingStrategy.Random;

    public sbyte[,] GuessingBoard { get; }

    public IList<(uint, uint)> Guesses { get; } = new List<(uint, uint)>();

    public Player(IList<Ship> ships)
    {
        _ships = ships;
        GuessingBoard = Initialize2DArray();
    }

    /// <summary>
    ///     Answers to another player's guess.
    /// </summary>
    /// <param name="guess">Coordinates on the board</param>
    /// <param name="shipType">Type of sunk ship or null</param>
    public BattleAnswer Answer((uint X, uint Y) guess, out ShipType? shipType)
    {
        shipType = null;
        Ship? ship = null;
        foreach (var sh in _ships)
        {
            var shipSegment = sh.Segments.FirstOrDefault(segment => segment.Coords == guess);

            if (shipSegment is null)
            {
                continue;
            }

            shipSegment.IsSunk = true;
            ship = sh;
            break;
        }

        if (ship is null)
        {
            return BattleAnswer.Miss;
        }

        if (_ships.SelectMany(s => s.Segments).All(seg => seg.IsSunk))
        {
            return BattleAnswer.HitAndWholeFleetSunk;
        }

        if (!ship.Segments.All(segment => segment.IsSunk))
        {
            return BattleAnswer.HitNotSunk;
        }

        shipType = ship.Type;
        return BattleAnswer.HitAndSunk;
    }

    public void ApplyAnswerInfo((uint X, uint Y) guess, BattleAnswer answer)
    {
        Guesses.Add(guess);

        switch (answer)
        {
            case BattleAnswer.Miss:
                GuessingBoard[guess.X, guess.Y] = 0;
                break;
            case BattleAnswer.HitAndWholeFleetSunk:
            case BattleAnswer.HitNotSunk:
                GuessingBoard[guess.X, guess.Y] = 1;
                break;
            case BattleAnswer.HitAndSunk:
                GuessingBoard[guess.X, guess.Y] = 1;
                MarkTilesAroundShipSegment(guess);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(answer), answer, $"Unknown {nameof(BattleAnswer)}");
        }
    }

    private void MarkTilesAroundShipSegment((uint X, uint Y) guess)
    {
        var (x, y) = ((int)guess.X, (int)guess.Y);
        GuessingBoard[x, y] = 3;

        foreach (var (i, j) in Constants.NeighborTilesRelativeCoords)
        {
            if (x + i >= Constants.BoardLength || x + i < 0 || y + j >= Constants.BoardLength || y + j < 0)
            {
                // tiles beyond the edge
                continue;
            }

            var value = GuessingBoard[x + i, y + j];
            if (value == 1)
            {
                MarkTilesAroundShipSegment(((uint)(x + i), (uint)(y + j)));
            }
            else if (value == -1)
            {
                GuessingBoard[x + i, y + j] = 2;
            }
        }
    }

    private static sbyte[,] Initialize2DArray(sbyte value = -1)
    {
        var board = new sbyte[10, 10];

        foreach (var row in Enumerable.Range(0, 10))
        {
            foreach (var col in Enumerable.Range(0, 10))
            {
                board[row, col] = value;
            }
        }

        return board;
    }
}
