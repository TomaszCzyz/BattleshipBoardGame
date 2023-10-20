using System.Text.Json;
using BattleshipBoardGame.Models;
using Microsoft.EntityFrameworkCore;

namespace BattleshipBoardGame.DbContext;

public interface ISimulationsDbContext
{
    DbSet<Simulation> Simulations { get; }
}

public class SimulationsDbContext : Microsoft.EntityFrameworkCore.DbContext, ISimulationsDbContext
{
    public DbSet<Simulation> Simulations => Set<Simulation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Player>()
            .Property(dto => dto.Board)
            .HasConversion(
                bytes => JsonSerializer.Serialize(bytes, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<sbyte[][]>(s, JsonSerializerOptions.Default) ?? Array.Empty<sbyte[]>());

        modelBuilder
            .Entity<Player>()
            .Property(dto => dto.Guesses)
            .HasConversion(
                guesses => JsonSerializer.Serialize(guesses, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<string[]>(s, JsonSerializerOptions.Default) ?? Array.Empty<string>());
    }
}
