using BattleshipBoardGame.Extensions;
using BattleshipBoardGame.Services;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BattleshipBoardGame.Tests.Unit.Services;

public class BoardGeneratorTests
{
    private readonly BoardGenerator _sut = new();
    private readonly ITestOutputHelper _testOutputHelper;

    public BoardGeneratorTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Generate_StrategySimple_ReturnsValidBoard()
    {
        // Arrange
        var strategy = Strategy.Simple;

        // Act
        var board = _sut.Generate(strategy);
        _testOutputHelper.WriteLine(board.PrintToString());

        // Assert
        board.Should().BeOfType<sbyte[,]>();
        board.EnumerateRows().Should().HaveCount(10);
        // number of tiles with ships is equal to 5+4+3+2+2+1+1=18
        board.EnumerateRows().SelectMany(bytes => bytes).Where(b => b == 1).Should().HaveCount(18);
    }

    // todo: write tests checking if there is no adjacent ships
}
