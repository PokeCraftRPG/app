namespace PokeCraft.Settings;

internal record OpenAuthenticationSettings
{
  public AccessTokenSettings AccessToken { get; set; } = new();

  public static OpenAuthenticationSettings Initialize(IConfiguration configuration)
  {
    OpenAuthenticationSettings settings = configuration.GetSection("OpenAuthentication").Get<OpenAuthenticationSettings>() ?? new();

    string? lifetimeSecondsValue = Environment.GetEnvironmentVariable("OPEN_AUTHENTICATION_ACCESS_TOKEN_LIFETIME");
    if (!string.IsNullOrWhiteSpace(lifetimeSecondsValue) && int.TryParse(lifetimeSecondsValue, out int lifetimeSeconds))
    {
      settings.AccessToken.LifetimeSeconds = lifetimeSeconds;
    }

    return settings;
  }
}

internal record AccessTokenSettings
{
  public int LifetimeSeconds { get; set; } = 300;
}
