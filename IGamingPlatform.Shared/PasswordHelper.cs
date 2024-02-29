using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IGamingPlatform.Shared;

public static class PasswordHelper
{
    public static (string hashedPassword, string salt) HashPassword(string password)
    {
        var salt = new byte[128 / 8];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return (hashedPassword, Convert.ToBase64String(salt));
    }

    public static bool VerifyPassword(string inputPassword, string storedPasswordHash, string storedSalt)
    {
        var salt = Convert.FromBase64String(storedSalt);
        var hashedInputPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: inputPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashedInputPassword == storedPasswordHash;
    }
}