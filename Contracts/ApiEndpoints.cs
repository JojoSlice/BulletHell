namespace Contracts;

public class ApiEndpoints
{
    private const string Root = "api";
    public static class HighScore
    {
        private const string Base = $"{Root}/highscores";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = Base;
        public const string Delete = $"{Base}/{{id}}";
    }
    public static class User
    {
        private const string Base = $"{Root}/users";
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = Base;
        public const string Delete = $"{Base}/{{id}}";
    }
}
