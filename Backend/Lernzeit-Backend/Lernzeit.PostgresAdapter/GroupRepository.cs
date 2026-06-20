using FunicularSwitch;
using Lernzeit.Application.Contracts;
using Lernzeit.Application.ResultTypes;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Entities;
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


    public async Task<List<Group>> GetAllGroups()
    {
        var groups = await context.Groups.ToListAsync();
        
        return groups.Select(g => g.ToDomain()).ToList();
    }

    public async Task<Option<Group>> GetGroupById(Guid id)
    {
        var group = await context.Groups
            .Include(g => g.UserGroups)
            .ThenInclude(ug => ug.User)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        return group.ToOption().Map(g => g.ToDomain());
    }

    public async Task<RepositoryResult<Unit>> CreateGroup(string groupName, GoogleUserId creatorId)
    {
        var user = (await context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == creatorId.Id)).ToOption().Map(u => u.ToDomain());
        if (user.IsNone())
        {
            return RepositoryResult.Error(RepositoryError.NotFound($"User with id {creatorId} not found"));
        }

        var newGroup = Group.Create(groupName, user.GetValueOrThrow());
        context.Groups.Add(newGroup.ToDbEntity());
        await context.SaveChangesAsync();

        return RepositoryResult.Ok(No.Thing);
    }

    public async Task<RepositoryResult<Unit>> AddUserToGroup(Guid userId, Guid groupId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
        {
            return RepositoryResult.Error(RepositoryError.NotFound($"User with id {userId.ToString()} not found"));
        }

        if (string.IsNullOrEmpty(user.Calendar))
        {
            return RepositoryResult.Error(RepositoryError.BadRequest($"User with id {userId.ToString()} has no calendar"));
        }

        if (!GroupExists(groupId))
            return RepositoryResult.Error(RepositoryError.NotFound($"Group with id {groupId.ToString()} not found"));
        
        var isAlreadyInGroup = await context.UserGroups.FindAsync(userId, groupId);
        if (isAlreadyInGroup != null)
        {
            return RepositoryResult.Error(RepositoryError.BadRequest($"User with id {userId.ToString()} is already in group with id {groupId.ToString()}"));
        }
        
        var userGroup = new UserGroupEntity(UserId: userId, GroupId: groupId);
        context.UserGroups.Add(userGroup);
        await context.SaveChangesAsync();
        return RepositoryResult.Ok(No.Thing);
    }

    public async Task<RepositoryResult<Unit>> RemoveUserFromGroup(Guid userId, Guid groupId)
    {
        var userGroup = await context.UserGroups.FindAsync(userId, groupId);
        if (userGroup == null)
        {
            return RepositoryResult.Error(
                RepositoryError.BadRequest(
                    $"User with id {userId.ToString()} not found in group with id {groupId.ToString()}"));
        }

        context.UserGroups.Remove(userGroup);
        await context.SaveChangesAsync();
        var remainingMembers = await context.UserGroups.CountAsync(ug => ug.GroupId == groupId);
        if (remainingMembers == 0)
        {
            var group = await context.Groups.FindAsync(groupId);
            if (group != null)
            {
                context.Groups.Remove(group);
                await context.SaveChangesAsync();
            }
        }
        return RepositoryResult.Ok(No.Thing);  
    }

    private bool GroupExists(Guid id) => context.Groups.Any(e => e.Id == id);
}