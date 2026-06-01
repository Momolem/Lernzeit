namespace Lernzeit.Domain;

public class UserNotFoundException(int userId) : Exception($"User with Id {userId} was not found.")
{
    
}