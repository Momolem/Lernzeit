using System.ComponentModel.DataAnnotations;

namespace Lernzeit.Domain;

public record User(Guid Id, GoogleUserId UserId, string Name, string CalUrl, string Calendar)
{
    public static User Create(string name, GoogleUserId googleUserId) => new(Guid.NewGuid(), googleUserId, name, string.Empty, string.Empty);
};