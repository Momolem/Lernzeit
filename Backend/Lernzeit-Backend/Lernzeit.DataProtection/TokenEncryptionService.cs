using Lernzeit.Application.Contracts;
using Microsoft.AspNetCore.DataProtection;

namespace Lernzeit.DataProtection;

public class TokenEncryptionService : ITokenEncryptionService
{
    private readonly IDataProtector _protector;

    public TokenEncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("RaumzeitToken");
    }

    public string Encrypt(string plaintext)
        => _protector.Protect(plaintext);

    public string Decrypt(string ciphertext)
        => _protector.Unprotect(ciphertext);
}