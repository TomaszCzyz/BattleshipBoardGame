using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models;
using BattleshipBoardGame.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable.NSubstitute;
using NSubstitute;
using Xunit;

namespace BattleshipBoardGame.Tests.Unit.Services;

public class BattleshipBoardGameSimulatorTests
{
    private readonly IBattleshipGameSimulator _sut;

    private readonly ILogger<BattleshipGameSimulator> _logger = Substitute.For<ILogger<BattleshipGameSimulator>>();
    private readonly ISimulationsDbContext _dbContext = Substitute.For<ISimulationsDbContext>();
    private readonly IBoardGenerator _boardGenerator = Substitute.For<IBoardGenerator>();
    private readonly IGuessingEngine _guessingEngine = Substitute.For<IGuessingEngine>();

    public BattleshipBoardGameSimulatorTests()
    {
        _sut = new BattleshipGameSimulator(_logger, _dbContext, _boardGenerator, _guessingEngine);
    }

    [Fact]
    public async Task Create_ValidGuid_PlayerAWins_RunsSimulation()
    {
        // Arrange
        var shipOfA = (0, 0);
        var shipOfB = (1, 1);
        var shipSegmentsOfA = new List<ShipSegment> { new() { Coords = shipOfA, IsSunk = false } };
        var shipSegmentsOfB = new List<ShipSegment> { new() { Coords = shipOfB, IsSunk = false } };
        var shipsOfPlayerA = new List<Ship> { new(ShipType.Submarine, shipSegmentsOfA) };
        var shipsOfPlayerB = new List<Ship> { new(ShipType.Submarine, shipSegmentsOfB) };
        var guid = Guid.NewGuid();
        var sim = new Simulation { Id = guid, IsFinished = false };
        var simulations = new List<Simulation> { sim };
        var tasksMock = simulations.AsQueryable().BuildMockDbSet();

        _boardGenerator.GenerateShips().Returns(shipsOfPlayerA, shipsOfPlayerB);
        _guessingEngine.Guess(Arg.Any<sbyte[,]>(), Arg.Any<GuessingStrategy>()).Returns(shipOfB, (9, 9));
        _dbContext.Simulations.Returns(tasksMock);
        _dbContext.Simulations.FindAsync(Arg.Is<Guid>(g => g == guid)).Returns(sim);

        // Act
        await _sut.Create(guid);

        // Assert
        _boardGenerator.Received(2).GenerateShips();
        _guessingEngine.Received(2).Guess(Arg.Any<sbyte[,]>(), Arg.Any<GuessingStrategy>());
        sim.Player1.Should().NotBeNull();
        sim.Player2.Should().NotBeNull();
        sim.Winner.Should().NotBeNull();
        sim.Winner!.Id.Should().Be(sim.Player1!.Id);
        _dbContext.Received(1).SaveChanges();
    }
}
