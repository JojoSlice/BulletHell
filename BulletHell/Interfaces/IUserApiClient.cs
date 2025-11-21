using System.Threading.Tasks;
using BulletHell.Models;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface för kommunikation med User API.
/// </summary>
public interface IUserApiClient
{
    /// <summary>
    /// Registrerar en ny användare.
    /// </summary>
    /// <param name="username">Användarnamn.</param>
    /// <param name="passwordHash">Hashat lösenord (BCrypt).</param>
    /// <returns>RegistrationResult med status och meddelande.</returns>
    Task<RegistrationResult> RegisterUserAsync(string username, string passwordHash);
}
