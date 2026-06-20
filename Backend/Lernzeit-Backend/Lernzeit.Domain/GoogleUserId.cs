namespace Lernzeit.Domain;

public record GoogleUserId(string Id)
{
    public override string ToString() => Id;
}