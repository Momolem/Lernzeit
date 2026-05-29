using Lernzeit.Domain;
using Lernzeit.PostgresAdapter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LernzeitBackend;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LernzeitDbContext _context;

    public UserController(LernzeitDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<List<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return user;
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(string name, string? calUrl)
    {
        var newUser = new User(Id: 0, Name: name, CalUrl: calUrl ?? "");
        // if calUrl provided, retrieve .ics
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
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
