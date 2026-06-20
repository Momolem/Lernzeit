using FunicularSwitch;
using Lernzeit.Application.Contracts;
using Lernzeit.Application.ResultTypes;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter;

public class GroupRepository : IGroupRepository
{
    private readonly LernzeitDbContext context;

    public GroupRepository(LernzeitDbContext context)
    {
        this.context = context;
    }


    public async Task<List<Group>> GetGroupsForUser(GoogleUserId googleUserId)
    {
        var user = await context.Users
            .Include(u => u.Groups)
            .ThenInclude(g => g.Members)
            .FirstOrDefaultAsync(u => u.GoogleUserId == googleUserId.Id);

        if (user == null)
        {
            return [];
        }
        
        return user.Groups.Select(g => g.ToDomain()).ToList();
    }

    public async Task<Option<Group>> GetGroupById(Guid id)
    {
        var group = await context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        return group.ToOption().Map(g => g.ToDomain());
    }

    public async Task<RepositoryResult<Unit>> CreateGroup(string groupName, GoogleUserId creatorId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == creatorId.Id);
        if (user == null)
        {
            return RepositoryResult.Error(RepositoryError.NotFound($"User with id {creatorId} not found"));
        }

        var newGroup = Group.Create(groupName, user.ToDomain());
        var newGroupEntity = newGroup.ToDbEntity();
        
        newGroupEntity.Members.Clear();
        newGroupEntity.Members.Add(user);
        
        context.Groups.Add(newGroupEntity);
        
        await context.SaveChangesAsync();

        return RepositoryResult.Ok(No.Thing);
    }

    public async Task<RepositoryResult<Unit>> AddUserToGroup(GoogleUserId userId, Guid groupId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == userId.Id);
        if (user == null)
        {
            return RepositoryResult.Error(RepositoryError.NotFound($"User with id {userId} not found"));
        }

        var group = await context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
            return RepositoryResult.Error(RepositoryError.NotFound($"Group with id {groupId.ToString()} not found"));
        
        if (group.Members.Any(m => m.GoogleUserId == userId.Id))
        {
            return RepositoryResult.Error(RepositoryError.BadRequest($"User with id {userId} is already in group with id {groupId.ToString()}"));
        }
        
        group.Members.Add(user);
        await context.SaveChangesAsync();
        return RepositoryResult.Ok(No.Thing);
    }

    public async Task<RepositoryResult<Unit>> RemoveUserFromGroup(Guid userId, Guid groupId)
    {
        var group = await context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
        {
            return RepositoryResult.Error(RepositoryError.NotFound($"Group with id {groupId.ToString()} not found"));
        }

        var user = group.Members.FirstOrDefault(m => m.Id == userId);
        if (user == null)
        {
            return RepositoryResult.Error(
                RepositoryError.BadRequest(
                    $"User with id {userId.ToString()} not found in group with id {groupId.ToString()}"));
        }

        group.Members.Remove(user);
        await context.SaveChangesAsync();
        
        if (group.Members.Count == 0)
        {
            context.Groups.Remove(group);
            await context.SaveChangesAsync();
        }
        return RepositoryResult.Ok(No.Thing);  
    }

    private bool GroupExists(Guid id) => context.Groups.Any(e => e.Id == id);
}