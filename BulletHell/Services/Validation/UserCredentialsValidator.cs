namespace BulletHell.Services.Validation;

public class UserCredentialsValidator
{
    private const int MinUsernameLength = 3;
    private const int MinPasswordLength = 6;

    public ValidationResult ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return ValidationResult.Failure("Användarnamn får inte vara tomt");
        }

        if (username.Length < MinUsernameLength)
        {
            return ValidationResult.Failure($"Användarnamn måste vara minst {MinUsernameLength} tecken");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ValidationResult.Failure("Lösenord får inte vara tomt");
        }

        if (password.Length < MinPasswordLength)
        {
            return ValidationResult.Failure($"Lösenord måste vara minst {MinPasswordLength} tecken");
        }

        return ValidationResult.Success();
    }

    public ValidationResult Validate(string username, string password)
    {
        var usernameResult = ValidateUsername(username);
        if (!usernameResult.IsValid)
        {
            return usernameResult;
        }

        return ValidatePassword(password);
    }
}
