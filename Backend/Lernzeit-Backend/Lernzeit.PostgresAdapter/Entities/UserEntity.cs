namespace Lernzeit.PostgresAdapter.Entities;

public record UserEntity(Guid Id, string Name = "", string CalUrl= "",string Calendar= "")
{
    public ICollection<UserGroupEntity> UserGroups { get; } = new List<UserGroupEntity>();
};