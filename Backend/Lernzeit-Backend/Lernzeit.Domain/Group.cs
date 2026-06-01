namespace Lernzeit.Domain;

public record Group(int Id, string Name, string? Calendar, IReadOnlyList<User> Members)
{
    public static Group Create(string name, string? calendar) => new(0, name, calendar, []);
}
    
    