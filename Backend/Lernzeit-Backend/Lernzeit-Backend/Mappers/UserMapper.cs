using Lernzeit.Domain;
using LernzeitBackend.DTO;
using LernzeitBackend.DTOs;

namespace LernzeitBackend.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
        => new(
            user.Id.ToString(),
            user.Name,
            user.CalUrl,
            user.Calendar
        );

    public static User ToDomain(this UserDto userDto)
        => new(Guid.Parse(userDto.Id), userDto.Name, userDto.CalUrl, userDto.Calendar);
    
    public static IReadOnlyList<TimetableEventDto> ToTimetableEvents(this Calendar calendar)
        => calendar.Events.Select(e => new TimetableEventDto(Guid.NewGuid().ToString(), e.Name, e.Start, e.End, null)).ToList();
}