using BattleshipBoardGame.Models;
using Microsoft.EntityFrameworkCore;

namespace BattleshipBoardGame.DbContext;

public class SimulationsDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private DbSet<Simulation> Simulations => Set<Simulation>();
}
