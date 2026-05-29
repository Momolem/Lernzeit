namespace Lernzeit.Domain;

public record Group(int Id = 0, string Name = "", string Calendar = "")
{
    public ICollection<UserGroups> UserGroups { get; } = new List<UserGroups>();
};
    
    