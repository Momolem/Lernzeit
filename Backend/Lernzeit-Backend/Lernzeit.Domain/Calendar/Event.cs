namespace Lernzeit.Domain.Calendar;

public record Event(
    string Name,
    DateTimeOffset Start,
    DateTimeOffset End
);