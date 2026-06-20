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
    public async Task<RepositoryResult<Unit>> CreateUser(string name, string email, string? calUrl, string? calendar)
    {
        var user = await this.context.Users.FindAsync(email);
        if (user != null)
        {
            return RepositoryResult.Error(RepositoryError.NotFound($"User with email {email} already exists"));
        }

        var newUser = User.Create(name, email, calUrl, calendar);
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
        var user = await this.context.Users.FindAsync(id);
        if (user == null)
        {
            return Result.Error("User not found");
        }

        var userGroups = context.UserGroups.Where(ug => ug.UserId == id);
        context.UserGroups.RemoveRange(userGroups);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Result.Ok(No.Thing);   
    }
}