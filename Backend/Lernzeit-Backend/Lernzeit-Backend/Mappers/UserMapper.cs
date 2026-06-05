using Lernzeit.Domain;
using LernzeitBackend.DTOs;

namespace LernzeitBackend.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
        => new(
            user.Id,
            user.Name,
            user.CalUrl,
            user.Calendar
        );

    public static User ToDomain(this UserDto userDto)
        => new(userDto.Id, userDto.Name, userDto.CalUrl, userDto.Calendar);
}