namespace Lernzeit.PostgresAdapter.Entities;

public record UserEntity(int Id = 0, string Name = "", string CalUrl= "",string Calendar= "")
{
    public ICollection<UserGroupEntity> UserGroups { get; } = new List<UserGroupEntity>();
};