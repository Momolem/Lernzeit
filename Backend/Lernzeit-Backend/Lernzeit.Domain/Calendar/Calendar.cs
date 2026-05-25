using Lernzeit.Domain.Calendar;

public record Calendar(
    CalendarType Type,
    string Name,
    IReadOnlyList<Event> Events);

public enum CalendarType
{
    Raumzeit,
    Manual
}