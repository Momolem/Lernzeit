using System.Security.Claims;
using FunicularSwitch;
using Lernzeit.RaumzeitAPI;
using LernzeitBackend.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RaumzeitService raumzeitService;

        public UserController(RaumzeitService raumzeitService)
        {
            this.raumzeitService = raumzeitService;
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
                from calendar in raumzeitService.GetPersonalCalendar(userId)
                select calendar.Events.Select(e => new TimetableEventDto(
                    Guid.NewGuid().ToString(),
                    e.Name,
                    e.Start,
                    e.End,
                    null
                )).ToList());

            return calendarResult.Match<IActionResult>(
                this.Ok,
                this.NotFound
            );

        }
    }
}