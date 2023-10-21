using BattleshipBoardGame.Models;
using BattleshipBoardGame.Services;
using FluentAssertions;
using Xunit;

namespace BattleshipBoardGame.Tests.Unit.Services;

public class GuessingEngineTests
{
    private readonly GuessingEngine _sut = new();

    [Fact]
    public void Guess_ValidPlayerState_ReturnsGuess()
    {
        // Arrange
        var player = new Player
        {
            Name = "TestPlayerName",
            GuessingStrategy = GuessingStrategy.Random,
            Board = Initialize2DArray(),
            Guesses = new List<(uint, uint)>()
        };

        // Act
        var guess = _sut.Guess(player);

        // Assert
        guess.X.Should().BeInRange(0, 10);
        guess.Y.Should().BeInRange(0, 10);
    }

    [Fact]
    public void Guess_NoFreeTilesLeft_Throws()
    {
        // Arrange
        var player = new Player
        {
            Name = "TestPlayerName",
            GuessingStrategy = GuessingStrategy.Random,
            Board = Initialize2DArray(1),
            Guesses = new List<(uint, uint)>()
        };

        // Act
        var act = () => _sut.Guess(player);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("All tiles on the guessing board were already guessed (Parameter 'board')");
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
