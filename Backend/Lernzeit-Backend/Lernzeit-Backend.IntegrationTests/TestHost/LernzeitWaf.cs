using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace Lernzeit_Backend.IntegrationTests.TestHost;

public class LernzeitWaf : WebApplicationFactory<Program>
{
    private static readonly PostgreSqlContainer DbContainer = new PostgreSqlBuilder("postgres:15-alpine").Build();
    
    private readonly string dbName = "db_" + Guid.NewGuid().ToString("n");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        DbContainer.StartAsync().GetAwaiter().GetResult();

        var connectionString = new Npgsql.NpgsqlConnectionStringBuilder(DbContainer.GetConnectionString())
        {
            Database = dbName
        }.ToString();

        builder.UseSetting("ConnectionStrings:postgres", connectionString);

        base.ConfigureWebHost(builder);
    }
}
