namespace Lernzeit.Domain;

public record Group(Guid Id, string Name, string? Calendar, IReadOnlyList<User> Members)
{
    public static Group Create(string name, string? calendar) => new(Guid.NewGuid(), name, calendar, []);
}
    
    