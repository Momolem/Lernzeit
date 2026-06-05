using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter;

public class UserRepository : IUserRepository
{
    private readonly LernzeitDbContext _context;
    public UserRepository(LernzeitDbContext context)
    {
        this._context = context;
    }
    public async Task<List<User>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        
        return users.Select(u => u.ToDomain()).ToList();
    }

    public async Task<User> GetUser(int id)
    {
        var user = await this._context.Users.FindAsync(id);
        if (user == null) throw new Exception("User not found");
        return user.ToDomain();    
    }

    public async Task CreateUser(int id, string username, string? calUrl, string? Calendar)
    {
        var newUser = new User(id, Name: username, calUrl, Calendar);
        _context.Users.Add(newUser.ToDbEntity());
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(User updatedUser)
    {
        var oldUser = await _context.Users.FindAsync(updatedUser.Id);
        if (oldUser == null) throw new Exception("User not found");

        _context.Entry(oldUser).CurrentValues.SetValues(updatedUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await this._context.Users.FindAsync(id);
        if (user == null) throw new Exception("User not found");

        var userGroups = _context.UserGroups.Where(ug => ug.UserId == id);
        _context.UserGroups.RemoveRange(userGroups);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}