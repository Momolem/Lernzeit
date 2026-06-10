namespace Lernzeit.PostgresAdapter.Entities;

public record GroupEntity(Guid Id, string Name = "", string Calendar = "")
{
    public IList<UserGroupEntity> UserGroups { get; } = new List<UserGroupEntity>();
};

