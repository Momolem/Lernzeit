using Lernzeit.Application.Contracts;
using Lernzeit.PostgresAdapter;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace LernzeitBackend.Controller;

[ApiController]
[Route("api/[controller]")]
public class GroupController : ControllerBase
{
    private readonly LernzeitDbContext _context;
    private readonly IGroupRepository groupRepository;

    public GroupController(LernzeitDbContext context, IGroupRepository groupRepository)
    {
        _context = context;
        this.groupRepository = groupRepository;
    }

    [HttpGet]
    public async Task<List<GroupDto>> GetGroups()
    {
        var groups = await this.groupRepository.GetAllGroups();
        return groups.Select(g => g.ToDto()).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetGroup(int id)
    {
        var group = await this.groupRepository.GetGroupById(id);
        if (group == null)
        {
            return this.NotFound();
        }
        
        return this.Ok(group.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> CreateGroup(int creatorId, string groupName)
    {
        await groupRepository.CreateGroup(groupName, creatorId);
        return this.Ok();
    }
    
    [HttpPut("join/{groupId}")]
    public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
    {
        await groupRepository.AddUserToGroup(groupId, userId);
        return this.Ok();
    }
    [HttpPut("leave/{groupId}")]
    public async Task<IActionResult> LeaveGroup(int groupId, int userId)
    {
        await groupRepository.RemoveUserFromGroup(groupId, userId);
        return this.Ok();
    }
}
