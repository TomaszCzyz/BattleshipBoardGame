using System.Globalization;
using BattleshipBoardGame;
using BattleshipBoardGame.DbContext;
using BattleshipBoardGame.Models.Api;
using BattleshipBoardGame.Models.Entities;
using BattleshipBoardGame.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;
using PlayerInfo = BattleshipBoardGame.Models.Entities.PlayerInfo;

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
builder.Services.AddScoped<IValidator<PlayerInfos>, PlayerInfosValidator>();
builder.Services.AddDbContext<ISimulationsDbContext, SimulationsDbContext>(options
    => options
        .UseSqlite($"Data Source={Path.Join(Path.GetTempPath(), "simulations.db")}")
        .UseLoggerFactory(new SerilogLoggerFactory()));
builder.Services.AddCors(options
    => options.AddPolicy(
        Constants.AllowedSpecificOrigins,
        policy => policy.WithOrigins("http://localhost:3000").AllowAnyHeader()));

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
            var simulation = await dbContext.Simulations
                .Include(simulation1 => simulation1.Player1)
                .Include(simulation1 => simulation1.Player2)
                .FirstOrDefaultAsync(s => s.Id == id);

            return simulation is null
                ? Results.NotFound($"The simulation with id {id} has not been found")
                : Results.Ok(simulation);
        }

        var sims = dbContext.Simulations.Select(s => s.Id);

        return Results.Ok(sims);
    });

app.MapPost(
    "/simulations/battleship/",
    async (
        [FromServices] IValidator<PlayerInfos> validator,
        [FromServices] IBattleshipGameSimulator simulator,
        [FromBody] PlayerInfos playerInfos,
        CancellationToken cancellationToken) =>
    {
        var validationResult = await validator.ValidateAsync(playerInfos, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var playerInfo1 = MapPlayerInfo(playerInfos.Infos![0]!);
        var playerInfo2 = MapPlayerInfo(playerInfos.Infos![1]!);

        var id = Guid.NewGuid();
        var simulation = new Simulation { Id = id, IsFinished = false };

        await simulator.Run(simulation, playerInfo1, playerInfo2, cancellationToken);

        return Results.Ok(id.ToString());
    });

app.UseCors(Constants.AllowedSpecificOrigins);

await app.RunAsync();

Log.CloseAndFlush();
return;

PlayerInfo MapPlayerInfo(BattleshipBoardGame.Models.Api.PlayerInfo playerInfo)
    => new()
    {
        Name = "DefaultName",
        GuessingStrategy = Enum.Parse<GuessingStrategy>(playerInfo.GuessingStrategy!, ignoreCase: true),
        ShipsPlacementStrategy = Enum.Parse<ShipsPlacementStrategy>(playerInfo.ShipsPlacementStrategy!, ignoreCase: true)
    };
