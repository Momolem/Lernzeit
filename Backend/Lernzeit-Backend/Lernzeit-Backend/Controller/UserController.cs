using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LernzeitBackend;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LernzeitDbContext _context;
    private readonly IUserRepository userRepository;

    public UserController(LernzeitDbContext context)
    {
        _context = context;
        this.userRepository = userRepository
    }

    [HttpGet]
    public async Task<List<UserDto>> GetUsers()
    {
        var users = await userRepository.GetAllUsers();
        return users.Select(u => u.ToDto()).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await userRepository.GetUserById(id);
        if (user == null) return NotFound();
        
        return this.Ok(user.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(string name, string? calUrl)
    {
        
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, string name)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var updated = user with
        {
            Name = !string.IsNullOrEmpty(name) ? name : user.Name
        };

        _context.Entry(user).CurrentValues.SetValues(updated);
        await _context.SaveChangesAsync();
        return Ok();
    }
    [HttpPut("calendar/{id}")]
    public async Task<ActionResult> UpdateCalUrl(int id, string calUrl)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        // if calUrl provided, retrieve .ics

        var updated = user with
        {
            CalUrl = !string.IsNullOrEmpty(calUrl) ? calUrl : user.CalUrl
        };

        _context.Entry(user).CurrentValues.SetValues(updated);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var userGroups = _context.UserGroups.Where(ug => ug.UserId == id);
        _context.UserGroups.RemoveRange(userGroups);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
