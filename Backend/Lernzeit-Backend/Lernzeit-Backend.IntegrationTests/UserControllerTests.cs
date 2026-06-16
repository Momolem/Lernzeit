using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Lernzeit_Backend.IntegrationTests.TestHost;
using LernzeitBackend.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Lernzeit.PostgresAdapter;
using Lernzeit.PostgresAdapter.Entities;

namespace Lernzeit_Backend.IntegrationTests;

public class UserControllerTests : IAsyncLifetime
{
    private HttpClient client = null!;
    private LernzeitWaf waf = null!;

    public ValueTask InitializeAsync()
    {
        try
        {
            waf = new LernzeitWaf();
            client = waf.CreateClient();
            return ValueTask.CompletedTask;
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await waf.DisposeAsync();
    }

    [Fact]
    public async Task GetUsers_ReturnsSuccess()
    {
        var response = await client.GetAsync("/api/User", TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>(cancellationToken: TestContext.Current.CancellationToken);
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUser_ReturnsUser_WhenExists()
    {
        var userId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var user = new UserEntity(userId, "Test User", "", "{}");
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await client.GetAsync($"/api/User/{userId}", TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: TestContext.Current.CancellationToken);
        userDto.Should().NotBeNull();
        userDto!.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await client.GetAsync($"/api/User/{Guid.NewGuid()}", TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_DeletesUser_WhenExists()
    {
        var userId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var user = new UserEntity(userId, "To Delete", "", "{}");
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/User/{userId.ToString()}");
            
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify it's gone
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var userInDb = await db.Users.FindAsync([userId], TestContext.Current.CancellationToken);
            userInDb.Should().BeNull();
        }
    }
    
    [Fact]
    public async Task UpdateUser_UpdatesUser_WhenExists()
    {
        var userId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var user = new UserEntity(userId, "Old Name", "", "{}");
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var updateDto = new UserDto(userId.ToString(), "New Name", "newUrl", "{}");
        var response = await System.Net.Http.Json.HttpClientJsonExtensions.PutAsJsonAsync(client, "/api/User", updateDto, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var userInDb = await db.Users.FindAsync([userId], TestContext.Current.CancellationToken);
            userInDb.Should().NotBeNull();
            userInDb!.Name.Should().Be("New Name");
            userInDb.CalUrl.Should().Be("newUrl");
        }
    }
}
