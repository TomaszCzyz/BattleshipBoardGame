using BattleshipBoardGame.DbContext;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BattleshipBoardGame.Tests.Integration;

public class BattleshipBoardGameTests : IClassFixture<CustomWebApplicationFactory<AssemblyMarker>>
{
    private readonly CustomWebApplicationFactory<AssemblyMarker> _factory;

    public BattleshipBoardGameTests(CustomWebApplicationFactory<AssemblyMarker> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_NewSimulation_StartsNewSimulation()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/simulations/battleship/", new StringContent(""));

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var content = await response.Content.ReadAsStringAsync();
        Guid.TryParse(content, out var guid).Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ISimulationsDbContext>();
        db.Simulations.Should().Contain(simulation => simulation.Id == guid);
    }
}
