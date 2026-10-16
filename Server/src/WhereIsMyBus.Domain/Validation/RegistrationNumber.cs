using System.Text.RegularExpressions;
namespace WhereIsMyBus.Domain.Validation;
public static partial class RegistrationNumber
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 30)
            throw new ArgumentException("Enter a bus registration number of at most 30 characters.");
        var normalized = value.Replace(" ", "").Replace("-", "").ToUpperInvariant();
        if (!Format().IsMatch(normalized))
            throw new ArgumentException("Use letters and numbers, for example TS09AB1234.");
        return normalized;
    }
    // Broad registration validation permits regional variants; stricter rules can be added per operator.
    [GeneratedRegex("^[A-Z0-9]{6,15}$", RegexOptions.CultureInvariant)]
    private static partial Regex Format();
}
