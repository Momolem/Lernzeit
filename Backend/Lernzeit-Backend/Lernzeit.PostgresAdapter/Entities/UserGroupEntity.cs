namespace Lernzeit.PostgresAdapter.Entities;

public record UserGroupEntity(
    int UserId,
    int GroupId
)
{
    public UserEntity? User { get; set; }
    public GroupEntity? Group { get; set; }
};