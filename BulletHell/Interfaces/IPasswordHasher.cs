namespace BulletHell.Interfaces;

/// <summary>
/// Interface för lösenordshashning och verifiering.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashar ett lösenord.
    /// </summary>
    /// <param name="password">Klartextlösenord.</param>.
    /// <returns>Hashat lösenord.</returns>.
    string HashPassword(string password);

    /// <summary>
    /// Verifierar ett lösenord mot en hash.
    /// </summary>
    /// <param name="password">Klartextlösenord.</param>.
    /// <param name="hash">Hashad sträng att jämföra mot.</param>.
    /// <returns>True om lösenordet matchar hashen.</returns>.
    bool VerifyPassword(string password, string hash);
}
