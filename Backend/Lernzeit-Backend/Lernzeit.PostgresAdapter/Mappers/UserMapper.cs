using System.Collections.Immutable;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Entities;

namespace Lernzeit.PostgresAdapter.Mappers;

public static class UserMapper
{
    public static User ToDomain(this UserEntity entity) => new(entity.Id, entity.Name, entity.Email, entity.CalUrl, entity.Calendar);

    public static UserEntity ToDbEntity(this User entity)
    {
        var userEntity = new UserEntity(
            entity.Id,
            entity.Name,
            entity.Email,
            entity.CalUrl,
            entity.Calendar);
        return userEntity;
    }
}