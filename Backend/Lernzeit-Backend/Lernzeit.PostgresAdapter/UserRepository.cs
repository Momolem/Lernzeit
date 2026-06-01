using Lernzeit.Application.Contracts;
using Lernzeit.Domain;

namespace Lernzeit.PostgresAdapter;

public class UserRepository : IUserRepository
{
    private readonly LernzeitDbContext _context;
    public UserRepository(LernzeitDbContext context)
    {
        _context = context;
    }
    public Task<List<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public Task CreateUser(string username, string? calUrl, string? Calendar)
    {
        var newUser = new User(Name: username, calUrl, Calendar);
        // if calUrl provided, retrieve .ics
        _context.Users.Add(newUser.ToDbEntity());
        await _context.SaveChangesAsync();
    }

    public Task UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(int id)
    {
        throw new NotImplementedException();
    }
}