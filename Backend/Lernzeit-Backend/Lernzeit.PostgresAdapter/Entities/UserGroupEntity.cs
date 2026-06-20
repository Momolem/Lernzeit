using Microsoft.EntityFrameworkCore;

namespace Lernzeit.PostgresAdapter.Entities;

[PrimaryKey(nameof(UserId), nameof(GroupId))]
public record UserGroupEntity(
    Guid UserId,
    Guid GroupId
)
{
    public UserEntity? User { get; set; }
    public GroupEntity? Group { get; set; }
};