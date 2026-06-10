namespace Lernzeit.PostgresAdapter.Entities;

public record UserGroupEntity(
    Guid UserId,
    Guid GroupId
)
{
    public UserEntity? User { get; set; }
    public GroupEntity? Group { get; set; }
};