using System.Text;
using BattleshipBoardGame.Extensions;
using BattleshipBoardGame.Models;
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
    public void GenerateShips_StrategySimple_ReturnsValidShipsList()
    {
        // Arrange
        var strategy = ShipsPlacementStrategy.Simple;

        // Act
        var ships = _sut.GenerateShips(strategy);
        _testOutputHelper.WriteLine(PrintShipsOnBoard(ships));

        // Assert
        ships.Should().HaveCount(7);
        ships.Where(ship => ship.Segments.Count == 1).Should().HaveCount(2);
        ships.Where(ship => ship.Segments.Count == 2).Should().HaveCount(2);
        ships.Where(ship => ship.Segments.Count == 3).Should().HaveCount(1);
        ships.Where(ship => ship.Segments.Count == 4).Should().HaveCount(1);
        ships.Where(ship => ship.Segments.Count == 5).Should().HaveCount(1);

        var shipSegments = ships.SelectMany(ship => ship.Segments).ToArray();
        var expectedNoOfSegments = 5 + 4 + 3 + 2 + 2 + 1 + 1;
        shipSegments.Select(s => s.Coords).Distinct().Should().HaveCount(expectedNoOfSegments); // no overlapping segments
        shipSegments.Should().HaveCount(expectedNoOfSegments);
        shipSegments.Select(s => s.IsSunk).Should().AllSatisfy(b => b.Should().BeFalse());
        shipSegments.Select(s => s.Coords).Should().AllSatisfy(
            b =>
            {
                b.X.Should().BeInRange(0, Constants.BoardLength);
                b.Y.Should().BeInRange(0, Constants.BoardLength);
            });
    }

    private static string PrintShipsOnBoard(IEnumerable<Ship> ships)
    {
        var coords = ships.SelectMany(s => s.Segments).Select(s => s.Coords).ToArray();

        var sb = new StringBuilder();

        for (var i = 0; i < Constants.BoardLength; i++)
        {
            for (var j = 0; j < Constants.BoardLength; j++)
            {
                sb.Append(coords.Contains((i, j)) ? '#' : '_');
            }

            sb.AppendLine();
        }

        sb.AppendLine();

        return sb.ToString();
    }
    // todo: write tests checking if there is no adjacent ships
}
