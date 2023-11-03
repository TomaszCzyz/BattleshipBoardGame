using System.Net;
using System.Net.Http.Json;
using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models.Api;
using BattleshipBoardGame.Models.Entities;
using BattleshipBoardGame.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using PlayerInfo = BattleshipBoardGame.Models.Api.PlayerInfo;

namespace BattleshipBoardGame.Tests.Integration;

public class BattleshipBoardGameTests : IClassFixture<CustomWebApplicationFactory<AssemblyMarker>>
{
    private readonly CustomWebApplicationFactory<AssemblyMarker> _factory;
    private readonly ISimulationsDbContext _dbContext;

    public BattleshipBoardGameTests(CustomWebApplicationFactory<AssemblyMarker> factory)
    {
        _factory = factory;
        var scope = _factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ISimulationsDbContext>();
    }

    [Fact]
    public async Task Get_RequestWithValidId_ReturnsSimulationData()
    {
        // Arrange
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();
        var testPlayerInfo = new Models.Entities.PlayerInfo
        {
            Name = "TestName",
            GuessingStrategy = GuessingStrategy.Random,
            ShipsPlacementStrategy = ShipsPlacementStrategy.Simple
        };
        var testSim = new Simulation
        {
            Id = id,
            IsFinished = false,
            Player1 = new PlayerDto
            {
                Id = Guid.NewGuid(),
                PlayerInfo = testPlayerInfo,
                Ships = new List<Ship> { new(ShipType.Carrier, new List<ShipSegment>()) },
                Guesses = new List<Point> { new(0, 0) }
            }
        };

        _dbContext.Simulations.Add(testSim);
        _dbContext.SaveChanges();

        // Act
        var response = await client.GetAsync($"/simulations/battleship/{id}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var content = await response.Content.ReadFromJsonAsync<Simulation>();

        content.Should().BeEquivalentTo(testSim);
    }

    [Fact]
    public async Task Get_RequestWithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var id = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/simulations/battleship/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be($"\"The simulation with id {id.ToString()} has not been found\"");
    }

    [Fact]
    public async Task Get_RequestWithoutId_ReturnsSimulationsList()
    {
        // Arrange
        var client = _factory.CreateClient();
        var guids = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToArray();
        var simulations = guids.Select(g => new Simulation { Id = g, IsFinished = true }).ToArray();

        _dbContext.Simulations.AddRange(simulations);
        _dbContext.SaveChanges();

        // Act
        var response = await client.GetAsync("/simulations/battleship/");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<string[]>();
        content.Should().Contain(s => guids.Select(g => g.ToString()).Contains(s));

        _dbContext.Simulations.RemoveRange(simulations);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Post_ValidRequest_RunsNewSimulation()
    {
        // Arrange
        var client = _factory.CreateClient();
        var playerInfos = new PlayerInfos
        {
            Infos = new PlayerInfo[]
            {
                new() { GuessingStrategy = "Random", ShipsPlacementStrategy = "Simple" },
                new() { GuessingStrategy = "Random", ShipsPlacementStrategy = "Simple" }
            }
        };

        // Act
        var response = await client.PostAsync("/simulations/battleship/", JsonContent.Create(playerInfos));

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var content = await response.Content.ReadAsStringAsync();

        Guid.TryParse(content, out var guid).Should().BeTrue();
        _dbContext.Simulations.Should().Contain(simulation => simulation.Id == guid);
    }

    [Fact]
    public async Task Post_InvalidStrategy_Returns()
    {
        // Arrange
        var invalidStrategyName = "InvalidStrategyName";
        var client = _factory.CreateClient();
        var playerInfos = new PlayerInfos
        {
            Infos = new PlayerInfo[]
            {
                new() { GuessingStrategy = invalidStrategyName, ShipsPlacementStrategy = "Simple" },
                new() { GuessingStrategy = "Random", ShipsPlacementStrategy = "Simple" }
            }
        };

        // Act
        var response = await client.PostAsync("/simulations/battleship/", JsonContent.Create(playerInfos));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(invalidStrategyName);
    }
}
