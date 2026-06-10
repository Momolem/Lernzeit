namespace LernzeitBackend.DTO
{
    public record TimetableEventDto(
        string Id,
        string Name,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        string? Room
    );
}
