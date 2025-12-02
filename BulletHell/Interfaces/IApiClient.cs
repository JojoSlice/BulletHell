using System.Threading.Tasks;
using BulletHell.Models;

namespace BulletHell.Interfaces;

/// <summary>
/// Interface för kommunikation med User API.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Registrerar en ny användare.
    /// </summary>
    /// <param name="username">Användarnamn.</param>
    /// <param name="passwordHash">Hashat lösenord (BCrypt).</param>
    /// <returns>RegistrationResult med status och meddelande.</returns>
    Task<RegistrationResult> RegisterUserAsync(string username, string passwordHash);

    /// <summary>
    /// Loggar in en användare.
    /// </summary>
    /// <param name="username">Användarnamn.</param>
    /// <param name="passwordHash">Hashat lösenord (BCrypt).</param>
    /// <returns>RegistrationResult med status och meddelande.</returns>
    Task<RegistrationResult> LoginAsync(string username, string passwordHash);

    /// <summary>
    /// Hämtar alla highscores.
    /// </summary>
    /// <returns>HighScoreListResult med alla highscores.</returns>
    Task<HighScoreListResult> GetAllHighScoresAsync();

    /// <summary>
    /// Hämtar en highscore med specifikt ID.
    /// </summary>
    /// <param name="id">Highscore ID.</param>
    /// <returns>HighScoreOperationResult med highscore data.</returns>
    Task<HighScoreOperationResult> GetHighScoreByIdAsync(int id);

    /// <summary>
    /// Skapar en ny highscore.
    /// </summary>
    /// <param name="score">Poäng.</param>
    /// <param name="userId">Användar-ID.</param>
    /// <returns>HighScoreOperationResult med den skapade highscoren.</returns>
    Task<HighScoreOperationResult> CreateHighScoreAsync(int score, int userId);

    /// <summary>
    /// Uppdaterar en befintlig highscore.
    /// </summary>
    /// <param name="id">Highscore ID.</param>
    /// <param name="score">Ny poäng.</param>
    /// <param name="userId">Användar-ID.</param>
    /// <returns>HighScoreOperationResult med den uppdaterade highscoren.</returns>
    Task<HighScoreOperationResult> UpdateHighScoreAsync(int id, int score, int userId);

    /// <summary>
    /// Tar bort en highscore.
    /// </summary>
    /// <param name="id">Highscore ID att ta bort.</param>
    /// <returns>HighScoreOperationResult med status.</returns>
    Task<HighScoreOperationResult> DeleteHighScoreAsync(int id);
}
