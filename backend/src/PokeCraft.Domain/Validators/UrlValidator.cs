using FluentValidation;
using FluentValidation.Validators;

namespace PokeCraft.Domain.Validators;

internal class UrlValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _schemes;

  public string Name { get; } = "UrlValidator";
  public IReadOnlyCollection<string> Schemes => _schemes.ToList().AsReadOnly();

  public UrlValidator(IEnumerable<string>? schemes = null)
  {
    _schemes = schemes?.Select(scheme => scheme.ToLowerInvariant()).ToHashSet() ?? ["http", "https"];
  }

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be a valid absolute Uniform Resource Locators (URL) using one of the following schemes: {string.Join(", ", Schemes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    try
    {
      Uri uri = new(value, UriKind.Absolute);
      return _schemes.Contains(uri.Scheme.ToLowerInvariant());
    }
    catch (Exception)
    {
      return false;
    }
  }
}
