using System.Text.Json;
using BattleshipBoardGame.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BattleshipBoardGame.DbContext;

public interface ISimulationsDbContext
{
    DbSet<Simulation> Simulations { get; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
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
                guesses => JsonSerializer.Serialize(guesses, new JsonSerializerOptions { IncludeFields = true, }),
                s => JsonSerializer.Deserialize<List<Point>>(s, new JsonSerializerOptions { IncludeFields = true, }) ?? new List<Point>(),
                new ValueComparer<IList<Point>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
    }
}
