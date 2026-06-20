using FunicularSwitch;
using Lernzeit.Application.ResultTypes;
using Lernzeit.Domain;

namespace Lernzeit.Application.Contracts;

public interface IGroupRepository
{
    public Task<List<Group>> GetAllGroups();
    public Task<Option<Group>> GetGroupById(Guid id);
    public Task<RepositoryResult<Unit>> CreateGroup(string groupName, GoogleUserId creatorId);
    public Task<RepositoryResult<Unit>> AddUserToGroup(Guid userId, Guid groupId);
    public Task<RepositoryResult<Unit>> RemoveUserFromGroup(Guid userId, Guid groupId);
}