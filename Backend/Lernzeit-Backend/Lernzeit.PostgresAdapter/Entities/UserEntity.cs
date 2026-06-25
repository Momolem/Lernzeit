namespace Lernzeit.PostgresAdapter.Entities;

public record UserEntity(Guid Id, string GoogleUserId, string Name = "", string CalUrl= "",string Calendar= "")
{
    public ICollection<GroupEntity> Groups { get; } = new List<GroupEntity>();
};