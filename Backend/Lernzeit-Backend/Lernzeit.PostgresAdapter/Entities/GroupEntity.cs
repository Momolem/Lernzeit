namespace Lernzeit.PostgresAdapter.Entities;

public record GroupEntity(Guid Id, string Name)
{
    public List<UserEntity> Members { get; init; } = new();
}
