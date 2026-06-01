using Lernzeit.Domain;

namespace Lernzeit.Application.Contracts;

public interface IGroupRepository
{
    public Task<List<Group>> GetAllGroups();
    public Task<Group?> GetGroupById(int id);
    public Task CreateGroup(string groupName, int creatorId);
    public Task AddUserToGroup(int userId, int groupId);
    public Task RemoveUserFromGroup(int userId, int groupId);
}