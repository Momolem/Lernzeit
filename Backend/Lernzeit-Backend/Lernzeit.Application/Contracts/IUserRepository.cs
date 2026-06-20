using FunicularSwitch;
using Lernzeit.Application.ResultTypes;
using Lernzeit.Domain;

namespace Lernzeit.Application.Contracts;

public interface IUserRepository
{
    public Task<List<User>> GetAllUsers();
    public Task<Result<User>> GetUser(Guid id);
    public Task<Result<Unit>> UpdateUser(User updatedUser);
    public Task<Result<Unit>> DeleteUser(Guid id);
    public Task<RepositoryResult<Unit>> CreateUserIfNotExists(GoogleUserId googleUserId, string name);
}