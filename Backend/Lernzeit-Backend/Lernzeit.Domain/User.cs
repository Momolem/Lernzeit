namespace Lernzeit.Domain;

public record User(Guid Id, string Name, string CalUrl, string Calendar)
{
};