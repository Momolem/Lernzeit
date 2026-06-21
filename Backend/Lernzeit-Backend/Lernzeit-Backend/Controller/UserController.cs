using System.Security.Claims;
using FunicularSwitch;
using Lernzeit.Application.Contracts;
using Lernzeit.RaumzeitAPI;
using LernzeitBackend.DTO;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend.Controller;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly RaumzeitService raumzeitService;

    public UserController(IUserRepository userRepository, RaumzeitService raumzeitService)
    {
        this.userRepository = userRepository;
        this.raumzeitService = raumzeitService;
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

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
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
    
    [Authorize]
    [HttpPost("raumzeit/login")]
    public async Task<IActionResult> Login([FromBody] RaumzeitLoginRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return this.Unauthorized();
        }
        await this.raumzeitService.Login(userId, request.Username, request.Password);
        return this.Ok();
    }

    [Authorize]
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar()
    {
        var calendarResult = await (
            from userId in User.FindFirstValue(ClaimTypes.NameIdentifier).ToOption()
                .ToResult(() => "UserID not found")
            from calendar in raumzeitService.GetPersonalCalendar(new Guid(userId))
            select calendar.ToTimetableEvents());

        return calendarResult.Match<IActionResult>(
            this.Ok,
            this.NotFound
        );

    }
}