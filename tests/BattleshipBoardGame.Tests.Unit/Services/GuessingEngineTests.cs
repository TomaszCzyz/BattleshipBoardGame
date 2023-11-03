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

    [Theory]
    [InlineData(5, 5)]
    [InlineData(2, 3)]
    [InlineData(7, 7)]
    [InlineData(0, 0)]
    [InlineData(9, 0)]
    [InlineData(0, 9)]
    [InlineData(9, 9)]
    public void Guess_OneFreeTileLeft_ReturnsTileCoords(int row, int col)
    {
        // Arrange
        var board = Array2dHelpers.Initialize(Constants.BoardLength, 0);
        board[row, col] = -1;

        // Act
        var guessRandom = _sut.Guess(board, GuessingStrategy.Random);
        var guessFromCenter = _sut.Guess(board, GuessingStrategy.FromCenter);

        // Assert
        var expected = new Point(row, col);
        guessRandom.Should().Be(expected);
        guessFromCenter.Should().Be(expected);
    }

    [Fact]
    public void Guess_FromCenterStrategy_ReturnsFirstClosestToCenter()
    {
        // Arrange
        var board = Array2dHelpers.Initialize(Constants.BoardLength);
        board[5, 5] = 0;
        board[4, 5] = 0;
        board[4, 4] = 0;
        board[5, 4] = 0;

        // Act
        var guessFromCenter = _sut.Guess(board, GuessingStrategy.FromCenter);

        // Assert
        guessFromCenter.Should().Be(new Point(6, 4));
    }
}
