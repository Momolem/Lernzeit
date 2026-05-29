namespace Lernzeit.PostgresAdapter.Entities;

public record GroupEntity(int Id = 0, string Name = "", string Calendar = "")
{
    public IList<UserGroupEntity> UserGroups { get; } = new List<UserGroupEntity>();
};

