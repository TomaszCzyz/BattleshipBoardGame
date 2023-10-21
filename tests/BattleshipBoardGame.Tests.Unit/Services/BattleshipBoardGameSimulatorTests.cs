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
    public async Task Create_ValidGuid_RunsNewSimulation()
    {
        // Arrange
        // use boardGenerator to create valid test data 
        var boardGenerator = new BoardGenerator();
        var guid = Guid.NewGuid();
        var sim = new Simulation { Id = guid, IsFinished = false };
        var simulations = new List<Simulation> { sim };
        var tasksMock = simulations.AsQueryable().BuildMockDbSet();

        _boardGenerator.Generate().Returns(boardGenerator.Generate(), boardGenerator.Generate());
        _dbContext.Simulations.Returns(tasksMock);

        // todo: how to mock rounds? maybe introduce smh like Arbiter class?

        // Act
        await _sut.Create(guid);

        // Assert
        sim.Player1.Should().NotBeNull();
        sim.Player2.Should().NotBeNull();
        _boardGenerator.Received(2).Generate();
        _dbContext.Received(1).SaveChanges();
    }
}
