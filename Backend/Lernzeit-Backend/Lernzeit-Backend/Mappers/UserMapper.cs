using Lernzeit.Domain;
using LernzeitBackend.DTOs;

namespace LernzeitBackend.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
        => new(
            user.Id,
            user.Name,
            user.Calendar
        );
}