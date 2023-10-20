#pragma warning disable CA1848
using System.Globalization;
using BattleshipBoardGame.DbContext;
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

builder.Services.AddDbContext<SimulationsDbContext>(options
    => options
        .UseSqlite($"Data Source={Path.Join(Path.GetTempPath(), "simulations.db")}")
        .UseLoggerFactory(new SerilogLoggerFactory()));

var app = builder.Build();
app.Logger.LogInformation("Application created. Launching application...");

await app.RunAsync();

Log.CloseAndFlush();
