namespace LernzeitBackend.DTOs;

public record GroupDto(
    string Id, 
    string Name,
    List<UserDto> Members);