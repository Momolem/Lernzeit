using Lernzeit.Domain;

namespace Lernzeit.Application.Contracts;

public interface IUserRepository
{
    public Task<List<User>> GetAllUsers();
    public Task<User> GetUserById(int id);
    public Task CreateUser(User user);
    public Task UpdateUser(User user);
    public Task DeleteUser(int id);
}