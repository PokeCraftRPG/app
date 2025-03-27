using Microsoft.Extensions.Configuration;

namespace PokeCraft.Application.Settings;

internal record AccountSettings
{
  public int WorldLimit { get; set; } = 3;

  public static AccountSettings Initialize(IConfiguration configuration)
  {
    AccountSettings settings = configuration.GetSection("Account").Get<AccountSettings>() ?? new();

    string? worldLimitValue = Environment.GetEnvironmentVariable("ACCOUNT_WORLD_LIMIT");
    if (!string.IsNullOrWhiteSpace(worldLimitValue) && int.TryParse(worldLimitValue, out int worldLimit))
    {
      settings.WorldLimit = worldLimit;
    }

    return settings;
  }
}
