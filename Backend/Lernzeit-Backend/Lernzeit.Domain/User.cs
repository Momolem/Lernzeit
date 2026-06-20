using System.ComponentModel.DataAnnotations;

namespace Lernzeit.Domain;

public record User(Guid Id, string Name, string Email, string CalUrl, string Calendar)
{
    public static User Create(string name, string email, string? calUrl, string? calendar) => new(Guid.NewGuid(), name, email, calUrl, calendar);
};