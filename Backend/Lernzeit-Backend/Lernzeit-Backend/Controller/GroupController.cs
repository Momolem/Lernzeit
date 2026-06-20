using Lernzeit.Application;
using Lernzeit.Application.Contracts;
using Lernzeit.Application.ResultTypes;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
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

    [HttpGet]
    public async Task<List<GroupDto>> GetGroups()
    {
        var groups = await this.groupRepository.GetAllGroups();
        return groups.Select(g => g.ToDto()).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetGroup(string id)
    {
        var group = await this.groupRepository.GetGroupById(Guid.Parse(id));
        return group.Match<ActionResult>(
            some: g => this.Ok(g.ToDto()),
            none: this.NotFound);
    }

    [HttpPost]
    public async Task<ActionResult> CreateGroup([FromBody] CreateGroupDto createGroupDto)
    {
        var createResult = await groupRepository.CreateGroup(createGroupDto.GroupName, creatorId: Guid.Parse(createGroupDto.CreatorId));
        createResult.Match(
            ok: _ => this.Ok(),
            error: MapRepositoryErrorToActionResult);
        return this.Ok();
    }

    [HttpPut("join/{groupId}")]
    public async Task<IActionResult> AddUserToGroup(string groupId, [FromBody] string userId)
    {
        var addResult = await groupRepository.AddUserToGroup(userId: Guid.Parse(userId), groupId: Guid.Parse(groupId));
        return addResult.Match(
            ok: _ => this.Ok(),
            error: MapRepositoryErrorToActionResult
        );
    }

    [HttpPut("leave/{groupId}")]
    public async Task<IActionResult> LeaveGroup(string groupId, [FromBody] string userId)
    {
        var removeResult = await groupRepository.RemoveUserFromGroup(userId: Guid.Parse(userId), groupId: Guid.Parse(groupId));
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