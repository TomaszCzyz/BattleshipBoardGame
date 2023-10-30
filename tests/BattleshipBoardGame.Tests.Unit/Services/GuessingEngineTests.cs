using BattleshipBoardGame.Helpers;
using BattleshipBoardGame.Models.Entities;
using BattleshipBoardGame.Services;
using FluentAssertions;
using Xunit;

namespace BattleshipBoardGame.Tests.Unit.Services;

public class GuessingEngineTests
{
    private readonly GuessingEngine _sut = new();

    [Fact]
    public void Guess_EmptyBoard_ReturnsAnyGuess()
    {
        // Arrange
        var board = Array2dHelpers.Initialize(Constants.BoardLength);

        // Act
        var guess = _sut.Guess(board, GuessingStrategy.Random);

        // Assert
        guess.Row.Should().BeInRange(0, 10);
        guess.Col.Should().BeInRange(0, 10);
    }

    [Fact]
    public void Guess_NoFreeTilesLeft_Throws()
    {
        // Arrange
        var board = Array2dHelpers.Initialize(Constants.BoardLength, 0);

        // Act
        var act = () => _sut.Guess(board, GuessingStrategy.Random);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("All tiles on the guessing board were already guessed (Parameter 'board')");
    }

    [Fact]
    public void Guess_OneFreeTileLeft_ReturnsTileCoords()
    {
        // Arrange
        var board = Array2dHelpers.Initialize(Constants.BoardLength, 0);
        board[2, 2] = -1;

        // Act
        var guess = _sut.Guess(board, GuessingStrategy.Random);

        // Assert
        guess.Should().Be((2, 2));
    }
}
