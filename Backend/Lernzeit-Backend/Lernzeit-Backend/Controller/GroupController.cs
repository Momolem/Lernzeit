using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter;
using LernzeitBackend.DTOs;
using LernzeitBackend.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var creator = _context.Users.Find(creatorId);
        if (creator == null) return BadRequest("User not found");
        if (string.IsNullOrEmpty(creator.Calendar)) return BadRequest("User has no calendar");
        
        var newGroup = new Group(Name: groupName, Calendar: creator.Calendar);
        _context.Groups.Add(newGroup);
        await _context.SaveChangesAsync();
        
        var userGroup = new UserGroups(UserId: creatorId, GroupId: newGroup.Id);
        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPut("join/{groupId}")]
    public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
    {
        var user = _context.Users.Find(userId);
        if (user == null) return BadRequest("User not found");
        if (string.IsNullOrEmpty(user.Calendar)) return BadRequest("User has no calendar");
        if (!GroupExists(groupId)) return BadRequest("Group not found");
        
        var isAlreadyInGroup = _context.UserGroups.Find(userId, groupId);
        if (isAlreadyInGroup != null) return Ok();
        
        var userGroup = new UserGroups(UserId: userId, GroupId: groupId);
        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    [HttpPut("leave/{groupId}")]
    public async Task<IActionResult> LeaveGroup(int groupId, int userId)
    {
        var userGroup = _context.UserGroups.Find(userId, groupId);
        if (userGroup == null) return BadRequest("User not in group");
        
        _context.UserGroups.Remove(userGroup);
        await _context.SaveChangesAsync();
        var remainingMembers = await _context.UserGroups.CountAsync(ug => ug.GroupId == groupId);
        if (remainingMembers == 0)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }
        
        return Ok();
    }
    
    private bool GroupExists(int id)
    {
        return _context.Groups.Any(e => e.Id == id);
    }
    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
