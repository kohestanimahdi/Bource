namespace Bource.Services.Security
{
    public interface IOneTimeToken
    {
        string GenerateToken(params string[] identifiers);

        bool ValidateToken(string token, params string[] identifiers);
    }
}