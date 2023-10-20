using BattleshipBoardGame.DbContext;
using JetBrains.Annotations;

namespace BattleshipBoardGame.Services;

public interface IBattleshipGameSimulator
{
    Task Create(Guid id);
}

[UsedImplicitly]
public class BattleshipGameSimulator : IBattleshipGameSimulator
{
    private readonly ILogger<BattleshipGameSimulator> _logger;
    private readonly ISimulationsDbContext _dbContext;

    public BattleshipGameSimulator(ILogger<BattleshipGameSimulator> logger, ISimulationsDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public Task Create(Guid id)
    {
        _logger.LogInformation("Starting new simulation with id {SimulationId}", id);

        return Task.CompletedTask;
    }
}
