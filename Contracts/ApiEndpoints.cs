namespace Contracts;

public class ApiEndpoints
{
    private const string _root = "api";
    public static class HighScore
    {
        private const string _base = $"{_root}/highscores";
        public const string GetAll = _base;
        public const string GetById = $"{_base}/{{id}}";
        public const string Create = _base;
        public const string Update = _base;
        public const string Delete = $"{_base}/{{id}}";
    }
    public static class User
    {
        private const string _base = $"{_root}/users";
        public const string GetAll = _base;
        public const string GetById = $"{_base}/{{id}}";
        public const string Create = _base;
        public const string Update = _base;
        public const string Delete = $"{_base}/{{id}}";
    }
}
