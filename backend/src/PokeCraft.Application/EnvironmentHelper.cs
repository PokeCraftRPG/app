namespace PokeCraft.Application;

public static class EnvironmentHelper
{
  public static string GetVariable(string variable, string defaultValue = "")
  {
    return TryGetVariable(variable) ?? defaultValue;
  }
  public static string? TryGetVariable(string variable, string? defaultValue = null)
  {
    string? value = Environment.GetEnvironmentVariable(variable);
    return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
  }
}
