using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;
using Lernzeit.PostgresAdapter;

namespace Lernzeit_Backend.IntegrationTests.TestHost;

public class LernzeitWaf : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder("postgres:15-alpine").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        base.ConfigureWebHost(builder);
    }

    public async ValueTask InitializeAsync()
    {
        await dbContainer.StartAsync();
        
        Environment.SetEnvironmentVariable("ConnectionStrings__postgres", dbContainer.GetConnectionString());

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
