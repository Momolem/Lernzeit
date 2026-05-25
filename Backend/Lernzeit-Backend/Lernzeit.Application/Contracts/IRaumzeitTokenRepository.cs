using FunicularSwitch;

namespace Lernzeit.Application.Contracts;

public interface IRaumzeitTokenRepository
{
    public Task<Result<string>> GetRaumzeitToken(string userId);
    
    public Task<Result<Unit>> SaveRaumzeitToken(string userId, string token, DateTimeOffset expiration);
}