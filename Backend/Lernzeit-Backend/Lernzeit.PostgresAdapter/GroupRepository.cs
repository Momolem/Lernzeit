using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
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

    public Task<Group> CreateGroup(Group group)
    {
        throw new NotImplementedException();
    }

    public Task AddUserToGroup(int userId, int groupId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUserFromGroup(int userId, int groupId)
    {
        throw new NotImplementedException();
    }
}