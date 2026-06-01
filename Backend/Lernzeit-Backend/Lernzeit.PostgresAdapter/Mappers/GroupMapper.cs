using System.Collections.Immutable;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Entities;

namespace Lernzeit.PostgresAdapter.Mappers;

public static class GroupMapper
{
    public static Group ToDomain(this GroupEntity entity) => new(
        entity.Id,
        entity.Name,
        entity.Calendar,
        entity.UserGroups.Select(s => s.User.ToDomain()).ToImmutableList()
    );    
    
    public static GroupEntity ToDbEntity(this Group entity)
    {
        var groupEntity = new GroupEntity(
            entity.Id,
            entity.Name,
            entity.Calendar ?? string.Empty);

        foreach (var userGroupEntity in entity.Members.Select(m => new UserGroupEntity(m.Id, entity.Id)))
        {
            groupEntity.UserGroups.Add(userGroupEntity);
        }
        
        return groupEntity;   
    }
}

public static class UserMapper
{
    public static User ToDomain(this UserEntity entity) => new(entity.Id, entity.Name, entity.CalUrl, entity.Calendar);
    
    public static UserEntity ToDbEntity(this User entity) => new(entity.Id, entity.Name, entity.Calendar);
}