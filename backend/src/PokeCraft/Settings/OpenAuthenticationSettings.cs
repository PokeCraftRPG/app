namespace PokeCraft.Settings;

internal record OpenAuthenticationSettings
{
  public AccessTokenSettings AccessToken { get; set; } = new();

  public static OpenAuthenticationSettings Initialize(IConfiguration configuration)
  {
    OpenAuthenticationSettings settings = configuration.GetSection("OpenAuthentication").Get<OpenAuthenticationSettings>() ?? new();

    string? accessTokenType = Environment.GetEnvironmentVariable("OPEN_AUTHENTICATION_ACCESS_TOKEN_TYPE");
    if (!string.IsNullOrWhiteSpace(accessTokenType))
    {
      settings.AccessToken.TokenType = accessTokenType;
    }

    string? lifetimeSeconds = Environment.GetEnvironmentVariable("OPEN_AUTHENTICATION_ACCESS_TOKEN_LIFETIME");
    if (!string.IsNullOrWhiteSpace(lifetimeSeconds))
    {
      settings.AccessToken.LifetimeSeconds = int.Parse(lifetimeSeconds);
    }

    return settings;
  }
}

internal record AccessTokenSettings
{
  public string TokenType { get; set; } = "at+jwt";
  public int LifetimeSeconds { get; set; } = 300;
}
