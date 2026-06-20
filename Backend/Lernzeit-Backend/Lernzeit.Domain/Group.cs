namespace Lernzeit.Domain;

public record Group(Guid Id, string Name, IReadOnlyList<User> Members)
{
    public static Group Create(string name, string? calendar) => new(Guid.NewGuid(), name, []);
}
    
    