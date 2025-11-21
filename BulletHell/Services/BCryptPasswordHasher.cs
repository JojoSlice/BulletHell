using System;
using BCrypt.Net;
using BulletHell.Interfaces;

namespace BulletHell.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(hash);

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }
}
