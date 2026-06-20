using Lernzeit.Domain;
using LernzeitBackend.DTOs;

namespace LernzeitBackend.Mappers;

public static class GroupMapper
{
    public static GroupDto ToDto(this Group group) 
        => new(
            group.Id.ToString(),
            group.Name,
            group.Members.Select(m => m.ToDto()).ToList());
}