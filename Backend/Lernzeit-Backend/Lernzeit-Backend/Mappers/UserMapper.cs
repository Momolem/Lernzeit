using Lernzeit.Domain;
using LernzeitBackend.DTO;
using LernzeitBackend.DTOs;

namespace LernzeitBackend.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
        => new(
            user.Id.ToString(),
            user.UserId.Id,
            user.Name,
            user.CalUrl,
            user.Calendar
        );

    public static User ToDomain(this UserDto userDto)
        => new(Guid.Parse(userDto.Id), new GoogleUserId(userDto.GoogleUserId), userDto.Name, userDto.CalUrl, userDto.Calendar);
    
    public static IReadOnlyList<TimetableEventDto> ToTimetableEvents(this Calendar calendar)
        => calendar.Events.Select(e => new TimetableEventDto(Guid.NewGuid().ToString(), e.Name, e.Start, e.End, null)).ToList();
}