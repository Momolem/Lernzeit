using FunicularSwitch;
using Lernzeit.Application.Contracts;

namespace Lernzeit.PostgresAdapter;

public class RaumzeitTokenRepository : IRaumzeitTokenRepository
{
    private readonly LernzeitDbContext dbContext;
    private readonly ITokenEncryptionService tokenEncryptionService;
    private readonly TimeProvider timeProvider;

    public RaumzeitTokenRepository(
        LernzeitDbContext dbContext, ITokenEncryptionService tokenEncryptionService, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.tokenEncryptionService = tokenEncryptionService;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<string>> GetRaumzeitToken(Guid userId)
    {
        var raumzeitTokenOption = (await dbContext.RaumzeitTokens.FindAsync(userId)).ToOption();
        return raumzeitTokenOption
            .ToResult(() => "Raumzeit Token not found")
            .Bind(rt =>
                {
                    if (rt.Expiration <= timeProvider.GetUtcNow())
                    {
                        return Result.Error("Raumzeit Token expired");
                    }

                    var token = rt.EncryptedToken;
                    return Result.Ok(tokenEncryptionService.Decrypt(token));
                }
            );
    }

    public async Task<Result<Unit>> SaveRaumzeitToken(Guid userId, string token, DateTimeOffset expiration)
        => await Result.Try(async () => await (await dbContext.RaumzeitTokens.FindAsync(userId)).ToOption()
                .Match(async dbObject =>
                {
                    dbObject.EncryptedToken = this.tokenEncryptionService.Encrypt(token);
                    dbObject.Expiration = expiration;
                    dbContext.Update(dbObject);
                    await dbContext.SaveChangesAsync();
                }, async () =>
                {
                    this.dbContext.RaumzeitTokens.Add(new RaumzeitToken()
                        {
                            UserId = userId.ToString(),
                            EncryptedToken = tokenEncryptionService.Encrypt(token),
                            Expiration = expiration
                        }
                    );
                    await dbContext.SaveChangesAsync();
                }),
            e => $"Token could not be saved: {e.Message}");
}