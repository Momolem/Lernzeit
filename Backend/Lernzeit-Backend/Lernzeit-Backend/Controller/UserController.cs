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
    private readonly IUserRepository userRepository;

    public UserController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpGet]
    public async Task<List<UserDto>> GetUsers()
    {
        var users = await this.userRepository.GetAllUsers();
        return users.Select(u => u.ToDto()).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await this.userRepository.GetUser(id);
        return this.Ok(user.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
    {
        var user = userDto.ToDomain();
        await this.userRepository.UpdateUser(user);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await this.userRepository.DeleteUser(id);
        return Ok();
    }
}
