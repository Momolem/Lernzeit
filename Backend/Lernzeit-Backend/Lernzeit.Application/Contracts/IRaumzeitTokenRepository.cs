using FunicularSwitch;

namespace Lernzeit.Application.Contracts;

public interface IRaumzeitTokenRepository
{
    public Task<Result<string>> GetRaumzeitToken(Guid userId);
    
    public Task<Result<Unit>> SaveRaumzeitToken(Guid userId, string token, DateTimeOffset expiration);
}