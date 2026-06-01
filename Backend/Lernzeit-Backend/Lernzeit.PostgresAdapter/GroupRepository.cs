using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Entities;
using Lernzeit.PostgresAdapter.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter;

public class GroupRepository : IGroupRepository
{
    private readonly LernzeitDbContext _context;

    public GroupRepository(LernzeitDbContext context)
    {
        this._context = context;
    }


    public async Task<List<Group>> GetAllGroups()
    {
        var groups = await _context.Groups.ToListAsync();
        
        return groups.Select(g => g.ToDomain()).ToList();
    }

    public async Task<Group?> GetGroupById(int id)
    {
        var group = await _context.Groups
            .Include(g => g.UserGroups)
            .ThenInclude(ug => ug.User)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return null;
        }
        
        return group.ToDomain();
    }

    public async Task CreateGroup(string groupName, int creatorId)
    {
        var creator = _context.Users.Find(creatorId);
        if (creator == null)
            throw new UserNotFoundException(creatorId);
        if (string.IsNullOrEmpty(creator.Calendar))
            throw new Exception("User has no calendar");

        var newGroup = Group.Create(groupName, creator.Calendar);
        _context.Groups.Add(newGroup.ToDbEntity());
        await _context.SaveChangesAsync();

        var userGroup = new UserGroupEntity(UserId: creatorId, GroupId: newGroup.Id);
        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();
    }

    public async Task AddUserToGroup(int userId, int groupId)
    {
        var user = _context.Users.Find(userId);
        if (user == null) throw new Exception("User not found");
        if (string.IsNullOrEmpty(user.Calendar)) throw new Exception("User has no calendar");
        if (!GroupExists(groupId)) throw new Exception("Group not found");
        
        var isAlreadyInGroup = _context.UserGroups.Find(userId, groupId);
        if (isAlreadyInGroup != null) return;
        
        var userGroup = new UserGroupEntity(UserId: userId, GroupId: groupId);
        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserFromGroup(int userId, int groupId)
    {
        var userGroup = _context.UserGroups.Find(userId, groupId);
        if (userGroup == null) throw new Exception("User not in group");
        
        _context.UserGroups.Remove(userGroup);
        await _context.SaveChangesAsync();
        var remainingMembers = await _context.UserGroups.CountAsync(ug => ug.GroupId == groupId);
        if (remainingMembers == 0)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }    }
    private bool GroupExists(int id)
    {
        return _context.Groups.Any(e => e.Id == id);
    }
}