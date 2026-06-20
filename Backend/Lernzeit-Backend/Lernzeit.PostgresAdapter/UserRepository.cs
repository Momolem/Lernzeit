using FunicularSwitch;
using Lernzeit.Application.ResultTypes;
using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Entities;
using Lernzeit.PostgresAdapter.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter;

public class UserRepository : IUserRepository
{
    private readonly LernzeitDbContext context;
    public UserRepository(LernzeitDbContext context)
    {
        this.context = context;
    }
    
    public async Task<RepositoryResult<Unit>> CreateUserIfNotExists(GoogleUserId googleUserId, string name)
    {
        var user = await this.context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == googleUserId.Id);
        if (user != null)
        {
            return RepositoryResult.Ok(No.Thing);
        }

        var newUser = User.Create(name, googleUserId);
        context.Users.Add(newUser.ToDbEntity());
        await context.SaveChangesAsync();
        return RepositoryResult.Ok(No.Thing);
    }
    public async Task<List<User>> GetAllUsers()
    {
        var users = await context.Users.ToListAsync();
        
        return users.Select(u => u.ToDomain()).ToList();
    }

    public async Task<Result<User>> GetUser(Guid id)
    {
        var user = await this.context.Users.FindAsync(id);
        if (user == null)
        {
            return Result.Error("User not found");
        }

        return Result.Ok(user.ToDomain());    
    }

    public async Task<Result<Unit>> UpdateUser(User updatedUser)
    {
        var oldUser = await context.Users.FindAsync(updatedUser.Id);
        
        if (oldUser == null)
        {
            return Result.Error("User not found");
        }

        context.Entry(oldUser).CurrentValues.SetValues(updatedUser);
        await context.SaveChangesAsync();
        return Result.Ok(No.Thing);
    }

    public async Task<Result<Unit>> DeleteUser(Guid id)
    {
        var user = await this.context.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return Result.Error("User not found");
        }

        user.Groups.Clear();
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Result.Ok(No.Thing);   
    }
}