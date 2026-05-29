namespace LernzeitBackend.DTOs;

public record GroupDto(
    int Id, 
    string Name,
    string Calendar,
    List<UserDto> Members);