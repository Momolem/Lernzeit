namespace Lernzeit.Domain;

public record Group(int Id, string Name, string? Calendar, IReadOnlyList<User> Members);
    
    