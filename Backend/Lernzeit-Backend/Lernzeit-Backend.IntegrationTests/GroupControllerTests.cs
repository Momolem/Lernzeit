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

[Collection("Database collection")]
public class GroupControllerTests
{
    private readonly HttpClient client;
    private readonly LernzeitWaf waf;

    public GroupControllerTests(LernzeitWaf waf)
    {
        this.waf = waf;
        client = waf.CreateClient();
    }

    [Fact]
    public async Task GetGroups_ReturnsEmpty_WhenNoGroupsExist()
    {
        var response = await client.GetAsync("/api/Group", TestContext.Current.CancellationToken);
        
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
            var user = new UserEntity(userId, "Test User", "", "{}");
            db.Users.Add(user);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await HttpClientJsonExtensions.PostAsJsonAsync(client, $"/api/Group", new { CreatorId = userId, GroupName = "TestGroup"}, TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroupById_ReturnsGroup_WhenExists()
    {
        var groupId = Guid.NewGuid();
        using (var scope = waf.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LernzeitDbContext>();
            var group = new GroupEntity(groupId, "ExistingGroup", "{}");
            db.Groups.Add(group);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await client.GetAsync($"/api/Group/{groupId}", TestContext.Current.CancellationToken);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroupById_ReturnsNotFound_WhenDoesNotExist()
    {
        var response = await client.GetAsync($"/api/Group/{Guid.NewGuid()}", TestContext.Current.CancellationToken);
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
            var user = new UserEntity(userId, "Test User", "", "{}");
            var group = new GroupEntity(groupId, "TestGroup", "{}");
            db.Users.Add(user);
            db.Groups.Add(group);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var response = await System.Net.Http.Json.HttpClientJsonExtensions.PutAsJsonAsync(client, $"/api/Group/join/{groupId}", userId.ToString(), TestContext.Current.CancellationToken);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
