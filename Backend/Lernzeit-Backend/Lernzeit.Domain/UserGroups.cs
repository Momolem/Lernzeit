namespace Lernzeit.Domain;

public record UserGroups(
    int UserId,
    int GroupId
)
{
    public User? User { get; set; }
    public Group? Group { get; set; }
}