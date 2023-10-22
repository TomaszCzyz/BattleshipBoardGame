using System.Text.Json;
using BattleshipBoardGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BattleshipBoardGame.DbContext;

public interface ISimulationsDbContext
{
    DbSet<Simulation> Simulations { get; }
    int SaveChanges();
}

public class SimulationsDbContext : Microsoft.EntityFrameworkCore.DbContext, ISimulationsDbContext
{
    public DbSet<Simulation> Simulations => Set<Simulation>();

    public SimulationsDbContext(DbContextOptions<SimulationsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<PlayerDto>()
            .OwnsOne(dto => dto.PlayerInfo);

        modelBuilder
            .Entity<PlayerDto>()
            .Property(dto => dto.Ships)
            .HasConversion(
                ships => JsonSerializer.Serialize(ships, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<List<Ship>>(s, JsonSerializerOptions.Default) ?? new List<Ship>(),
                new ValueComparer<IList<Ship>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        modelBuilder
            .Entity<PlayerDto>()
            .Property(dto => dto.Guesses)
            .HasConversion(
                guesses => JsonSerializer.Serialize(guesses, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<List<(int, int)>>(s, JsonSerializerOptions.Default) ?? new List<(int, int)>(),
                new ValueComparer<IList<(int, int)>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
    }
}
