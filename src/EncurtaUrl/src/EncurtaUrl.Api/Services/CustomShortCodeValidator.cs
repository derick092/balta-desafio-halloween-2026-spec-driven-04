namespace EncurtaUrl.Api.Services;

public interface ICustomShortCodeValidator
{
    string? GetValidationError(string code);
}

public class CustomShortCodeValidator : ICustomShortCodeValidator
{
    private static readonly string[] ReservedCodes = ["urls", "openapi"];

    public string? GetValidationError(string code)
    {
        if (code.Length is < 3 or > 32 ||
            !code.All(character => char.IsAsciiLetterOrDigit(character) || character is '-' or '_'))
        {
            return "O código deve conter de 3 a 32 caracteres: letras sem acento, números, hífen ou underscore.";
        }

        if (ReservedCodes.Contains(code, StringComparer.OrdinalIgnoreCase))
        {
            return "Este código é reservado para uma rota do serviço e não pode ser utilizado.";
        }

        return null;
    }
}
