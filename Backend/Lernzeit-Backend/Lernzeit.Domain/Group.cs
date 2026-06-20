namespace Lernzeit.Domain;

public record Group(Guid Id, string Name, IReadOnlyList<User> Members)
{
    public static Group Create(string name, User creator) => new(Guid.NewGuid(), name, [creator]);
}