namespace LernzeitBackend.DTOs;

public record GroupDto(
    string Id, 
    string Name,
    string Calendar,
    List<UserDto> Members);