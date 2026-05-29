namespace Lernzeit.Domain;

public record User(int Id = 0, string Name = "", string CalUrl= "",string Calendar= "")
{
    public ICollection<UserGroups> UserGroups { get; } = new List<UserGroups>();
};