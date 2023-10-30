using BattleshipBoardGame.Helpers;
using BattleshipBoardGame.Models.Entities;

namespace BattleshipBoardGame.Models;

/// <summary>
///     A class representing a player and its game preferences.
///     It is responsible for tracking his game progress and
///     updating its state based on information from opponent.
/// </summary>
public class Player
{
    private readonly IList<Ship> _ships;

    /// <summary>
    ///     A list of player's ships for reference.
    /// </summary>
    public IReadOnlyList<Ship> Ships => _ships.AsReadOnly();

    /// <summary>
    ///     The guessing strategy of the player.
    ///     Note, that the guessing itself is realised by different class
    ///     so the strategy might change during the game.
    /// </summary>
    public GuessingStrategy GuessingStrategy { get; } = GuessingStrategy.Random;

    /// <summary>
    ///     Keeps track of guessed coords, hit ships and coords that cannot contain ship
    /// </summary>
    public sbyte[,] GuessingBoard { get; }

    /// <summary>
    ///     Guesses of the player in an order (first guess at the index 0)
    /// </summary>
    public IList<Point> Guesses { get; } = new List<Point>();

    /// <summary>
    ///     Initializes a player with a given list of ships and a <see cref="GuessingBoard"/>
    ///     with all tiles set to 'unknown' state
    /// </summary>
    public Player(IList<Ship> ships)
    {
        _ships = ships;
        GuessingBoard = Array2dHelpers.Initialize(Constants.BoardLength);
    }

    /// <summary>
    ///     Answers to another player's guess.
    /// </summary>
    /// <param name="guess">Coordinates on the board</param>
    /// <param name="shipType">Type of a sunk ship or null</param>
    public PlayerAnswer Answer(Point guess, out ShipType? shipType)
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
            return PlayerAnswer.Miss;
        }

        if (_ships.SelectMany(s => s.Segments).All(seg => seg.IsSunk))
        {
            return PlayerAnswer.HitAndWholeFleetSunk;
        }

        if (!ship.Segments.All(segment => segment.IsSunk))
        {
            return PlayerAnswer.HitNotSunk;
        }

        shipType = ship.Type;
        return PlayerAnswer.HitAndSunk;
    }

    /// <summary>
    ///     Updates progress using received answer.
    /// </summary>
    /// <param name="guess">a guess to which we have an answer</param>
    /// <param name="answer">the answer to our guess</param>
    /// <exception cref="ArgumentOutOfRangeException">when we receive an unknown answer</exception>
    public void ApplyAnswerInfo(Point guess, PlayerAnswer answer)
    {
        Guesses.Add(guess);

        switch (answer)
        {
            case PlayerAnswer.Miss:
                GuessingBoard[guess.Row, guess.Col] = 0;
                break;
            case PlayerAnswer.HitAndWholeFleetSunk:
            case PlayerAnswer.HitNotSunk:
                GuessingBoard[guess.Row, guess.Col] = 1;
                break;
            case PlayerAnswer.HitAndSunk:
                GuessingBoard[guess.Row, guess.Col] = 1;
                MarkTilesAroundShipSegment(guess);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(answer), answer, $"Unknown {nameof(PlayerAnswer)}");
        }
    }

    private void MarkTilesAroundShipSegment(Point guess)
    {
        var (x, y) = (guess.Row, guess.Col);
        // mark current coords with different value, to avoid infinite recursion
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
                MarkTilesAroundShipSegment(new Point(x + i, y + j));
            }
            else if (value == -1)
            {
                GuessingBoard[x + i, y + j] = 2;
            }
        }
    }
}
