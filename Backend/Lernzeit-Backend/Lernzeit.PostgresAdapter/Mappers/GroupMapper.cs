using System.Collections.Immutable;
using Lernzeit.Domain;
using Lernzeit.PostgresAdapter.Entities;

namespace Lernzeit.PostgresAdapter.Mappers;

public static class GroupMapper
{
    public static Group ToDomain(this GroupEntity entity) => new(
        entity.Id,
        entity.Name,
        entity.Members?.Select(m => m.ToDomain()).ToImmutableList() ?? ImmutableList<User>.Empty
    );    
    
    public static GroupEntity ToDbEntity(this Group entity)
    {
        var groupEntity = new GroupEntity(
            entity.Id,
            entity.Name)
        {
            Members = entity.Members.Select(m => m.ToDbEntity()).ToList()
        };
        
        return groupEntity;   
    }
}