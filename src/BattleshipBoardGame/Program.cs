using System.Globalization;
using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models;
using BattleshipBoardGame.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, configuration) =>
    configuration
        .MinimumLevel.Information()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}]{NewLine} {Message:lj}{NewLine}{Exception}",
            formatProvider: CultureInfo.CurrentCulture));

builder.Services.AddDbContext<ISimulationsDbContext, SimulationsDbContext>(options
    => options
        .UseSqlite($"Data Source={Path.Join(Path.GetTempPath(), "simulations.db")}")
        .UseLoggerFactory(new SerilogLoggerFactory()));

var app = builder.Build();
app.Logger.LogInformation("Application created. Launching application...");

app.MapPost(
    "/simulations/battleship/new/",
    async (ISimulationsDbContext dbContext, IBattleshipGameSimulator simulator, CancellationToken cancellationToken) =>
    {
        var id = Guid.NewGuid();
        await dbContext.Simulations.AddAsync(new Simulation { Id = id, IsFinished = false }, cancellationToken);
        _ = simulator.Create(id);
        return id;
    });

await app.RunAsync();

Log.CloseAndFlush();
