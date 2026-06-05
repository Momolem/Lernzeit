using Lernzeit.Domain;

namespace Lernzeit.Application.Contracts;

public interface IUserRepository
{
    public Task<List<User>> GetAllUsers();
    public Task<User> GetUser(int id);
    public Task UpdateUser(User updatedUser);
    public Task DeleteUser(int id);
}