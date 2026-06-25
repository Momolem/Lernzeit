using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Lernzeit_Backend.IntegrationTests.TestHost;
using LernzeitBackend.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Lernzeit.PostgresAdapter;
using Lernzeit.PostgresAdapter.Entities;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace Lernzeit_Backend.IntegrationTests;

public class GroupControllerTests : IAsyncLifetime
{
    private HttpClient client = null!;
    private LernzeitWaf waf = null!;

    public async ValueTask InitializeAsync()
    {
        this.waf = new LernzeitWaf();
        client = waf.CreateClient();
    }

    public async ValueTask DisposeAsync()
    {
        await waf.DisposeAsync();
    }

    [Fact]
    public async Task GetGroups_ReturnsEmpty_WhenNoGroupsExist()
    {
        var userId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var user = new UserEntity(userId, "test-google-id", "Test User", "{}");
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await client.GetAsync("api/Group", TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var groups = await response.Content.ReadFromJsonAsync<List<GroupDto>>(cancellationToken: TestContext.Current.CancellationToken);
        groups.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateGroup_CreatesNewGroup()
    {
        var userId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var user = new UserEntity(userId, "test-google-id", "Test User", "{}");
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await HttpClientJsonExtensions.PostAsJsonAsync(client, "api/Group", new { GroupName = "TestGroup"}, TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            (await db.Groups.FirstOrDefaultAsync(g => g.Name == "TestGroup", cancellationToken: TestContext.Current.CancellationToken)).Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetGroupById_ReturnsGroup_WhenExists()
    {
        var groupId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var group = new GroupEntity(groupId, "ExistingGroup");
            db.Groups.Add(group);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await client.GetAsync($"api/Group/{groupId}", TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var groupDto = await response.Content.ReadFromJsonAsync<GroupDto>(cancellationToken: TestContext.Current.CancellationToken);
        groupDto.Should().NotBeNull();
        groupDto.Name.Should().Be("ExistingGroup");
    }

    [Fact]
    public async Task GetGroupById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await client.GetAsync($"api/Group/{Guid.NewGuid()}", TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task JoinGroup_ReturnsOk()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var user = new UserEntity(userId, "test-google-id", "Test User", "{}");
            var group = new GroupEntity(groupId, "TestGroup");
            db.Users.Add(user);
            db.Groups.Add(group);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await client.PostAsync($"api/Group/join/{groupId}", null, TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var groupDto = await client.GetFromJsonAsync<GroupDto>($"api/Group/{groupId}",
            cancellationToken: TestContext.Current.CancellationToken);
        groupDto.Should().NotBeNull();
        groupDto.Members.Should().ContainSingle(m => m == "Test User");
    }
}
