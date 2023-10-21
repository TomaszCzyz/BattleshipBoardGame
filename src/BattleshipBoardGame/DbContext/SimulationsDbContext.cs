using System.Text.Json;
using BattleshipBoardGame.Models;
using BattleshipBoardGame.Serialization;
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
            .Entity<Player>()
            .Property(dto => dto.Board)
            .HasConversion(
                bytes => JsonSerializer.Serialize(bytes, new JsonSerializerOptions { Converters = { new ArraySByte2DConverter() } }),
                s => JsonSerializer.Deserialize<sbyte[,]>(s, new JsonSerializerOptions { Converters = { new ArraySByte2DConverter() } })
                     ?? new sbyte[0, 0]);

        modelBuilder
            .Entity<Player>()
            .Property(dto => dto.Guesses)
            .HasConversion(
                guesses => JsonSerializer.Serialize(guesses, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<List<(uint, uint)>>(s, JsonSerializerOptions.Default) ?? new List<(uint, uint)>(),
                new ValueComparer<IList<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
    }
}
