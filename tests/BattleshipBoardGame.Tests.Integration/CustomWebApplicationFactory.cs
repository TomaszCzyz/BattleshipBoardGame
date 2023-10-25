using System.Data.Common;
using BattleshipBoardGame.DbContext;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BattleshipBoardGame.Tests.Integration;

[UsedImplicitly]
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder
            .UseEnvironment("Development")
            .ConfigureLogging(logging => logging.ClearProviders())
            .ConfigureServices(services =>
            {
                var dbContextDescriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<SimulationsDbContext>));
                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));

                services.Remove(dbContextDescriptor);
                services.Remove(dbConnectionDescriptor!);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(_ =>
                {
                    var testDbPath = Path.Combine(Path.GetTempPath(), "BattleshipBoardGameIntegrationTests.db");
                    var connection = new SqliteConnection($"DataSource={testDbPath}");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<ISimulationsDbContext, SimulationsDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
            });
}
