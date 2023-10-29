using System.Globalization;
using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models.Entities;
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

builder.Services.AddScoped<IGuessingEngine, GuessingEngine>();
builder.Services.AddScoped<IBoardGenerator, BoardGenerator>();
builder.Services.AddScoped<IBattleshipGameSimulator, BattleshipGameSimulator>();
builder.Services.AddDbContext<ISimulationsDbContext, SimulationsDbContext>(options
    => options
        .UseSqlite($"Data Source={Path.Join(Path.GetTempPath(), "simulations.db")}")
        .UseLoggerFactory(new SerilogLoggerFactory()));

var app = builder.Build();
app.Logger.LogInformation("Application created. Launching application...");

using var scope = app.Services.CreateScope();
await using (var db = scope.ServiceProvider.GetRequiredService<SimulationsDbContext>())
{
    app.Logger.LogInformation("Executing EF migrations...");
    db.Database.Migrate();
}

app.MapGet(
    "/simulations/battleship/{id:guid?}",
    async (ISimulationsDbContext dbContext, Guid? id) =>
    {
        if (id is not null)
        {
            var simulation = await dbContext.Simulations.FindAsync(id);
            return simulation is null
                ? Results.NotFound($"The simulation with id {id} has not been found")
                : Results.Ok(simulation);
        }

        var sims = dbContext.Simulations.Select(s => s.Id);

        return Results.Ok(sims);
    });

app.MapPost(
    "/simulations/battleship/",
    async (ISimulationsDbContext dbContext, IBattleshipGameSimulator simulator, CancellationToken cancellationToken) =>
    {
        var id = Guid.NewGuid();
        await dbContext.Simulations.AddAsync(new Simulation { Id = id, IsFinished = false }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        _ = simulator.Create(id);
        return id.ToString();
    });

await app.RunAsync();

Log.CloseAndFlush();
