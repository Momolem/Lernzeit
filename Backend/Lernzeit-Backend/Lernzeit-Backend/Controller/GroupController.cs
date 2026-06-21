using Lernzeit.Application;
using System.Security.Claims;
using FunicularSwitch;
using Lernzeit.Application.Contracts;
using Lernzeit.Application.ResultTypes;
using Lernzeit.Domain;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace LernzeitBackend.Controller;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IGroupRepository groupRepository;
    private readonly GroupCalendarService groupCalendarService;

    public GroupController(IGroupRepository groupRepository, GroupCalendarService groupCalendarService)
    {
        this.groupRepository = groupRepository;
        this.groupCalendarService = groupCalendarService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return NotFound("User not logged in");
        }

        var groups = await this.groupRepository.GetGroupsForUser(new GoogleUserId(userId));
        return this.Ok(groups.Select(g => g.ToDto()).ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetGroup(string id)
    {
        var group = await this.groupRepository.GetGroupById(Guid.Parse(id));
        return group.Match<ActionResult>(
            some: g => this.Ok(g.ToDto()),
            none: this.NotFound);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto createGroupDto)
    {
        var googleUserIdOption =
            User.FindFirstValue(ClaimTypes.NameIdentifier).ToOption().Map(g => new GoogleUserId(g));
        return await googleUserIdOption.Match<IActionResult>(async googleUserId =>
            {
                var createResult =
                    await groupRepository.CreateGroup(createGroupDto.GroupName, creatorId: googleUserId);
                return createResult.Match(
                    ok: _ => this.Ok(),
                    error: MapRepositoryErrorToActionResult);
            },
            () => this.BadRequest("User not logged in")
        );
    }

    [Authorize]
    [HttpPost("join/{groupId}")]
    public async Task<IActionResult> AddUserToGroup(string groupId)
    {
        var userIdOption = User.FindFirstValue(ClaimTypes.NameIdentifier)
            .ToOption()
            .Map(g => new GoogleUserId(g));

        return await userIdOption
            .Match(async userId => await groupRepository.AddUserToGroup(userId: userId, groupId: Guid.Parse(groupId)),
                () => RepositoryResult.Error(RepositoryError.BadRequest("User not logged in")))
            .Match(ok: _ => this.Ok(),
                error: MapRepositoryErrorToActionResult
            );
    }

    [HttpPost("leave/{groupId}")]
    public async Task<IActionResult> LeaveGroup(string groupId, [FromBody] string userId)
    {
        var removeResult =
            await groupRepository.RemoveUserFromGroup(userId: Guid.Parse(userId), groupId: Guid.Parse(groupId));
        return removeResult.Match(
            ok: _ => this.Ok(),
            error: MapRepositoryErrorToActionResult);
    }

    [HttpGet("{groupId}/calendar")]
    public async Task<IActionResult> GetGroupCalendar(string groupId)
    {
        var calendar = await groupCalendarService.GetGroupCalendar(new Guid(groupId));
        return calendar.Match<IActionResult>(
            some: cal => this.Ok(cal.ToTimetableEvents()),
            none: this.NotFound);
    }
    
    
    private IActionResult MapRepositoryErrorToActionResult(RepositoryError e)
        => e.Match<IActionResult>(
            notFound: nf => this.NotFound(nf.Message),
            badRequest: br => this.BadRequest(br.Message)
        );
}