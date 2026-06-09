using Lernzeit.Application.Contracts;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend.Controller;

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
    public async Task<IActionResult> GetUsers()
    {
        var users = await this.userRepository.GetAllUsers();
        return this.Ok(users.Select(u => u.ToDto()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await this.userRepository.GetUser(Guid.Parse(id));
        return user.Match<IActionResult>(
            ok: u => this.Ok(u.ToDto()),
            error: this.NotFound);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto userDto)
    {
        var updateResult = await this.userRepository.UpdateUser(userDto.ToDomain());
        return updateResult.Match<IActionResult>(
            ok: _ => this.Ok(),
            error: this.NotFound
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var removeResult = await this.userRepository.DeleteUser(Guid.Parse(id));
        return removeResult.Match<IActionResult>(
            ok: _ => this.Ok(),
            error: this.NotFound);
    }
}