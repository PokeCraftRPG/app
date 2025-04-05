using Microsoft.Extensions.Configuration;

namespace PokeCraft.Application.Settings;

internal record AccountSettings
{
  public long AllocatedBytes { get; set; } = 5 * 1024 * 1024; // 5 MB
  public int WorldLimit { get; set; } = 3;

  public static AccountSettings Initialize(IConfiguration configuration)
  {
    AccountSettings settings = configuration.GetSection("Account").Get<AccountSettings>() ?? new();

    string? allocatedBytesValue = Environment.GetEnvironmentVariable("ACCOUNT_ALLOCATED_BYTES");
    if (!string.IsNullOrWhiteSpace(allocatedBytesValue) && long.TryParse(allocatedBytesValue, out long allocatedBytes))
    {
      settings.AllocatedBytes = allocatedBytes;
    }

    string? worldLimitValue = Environment.GetEnvironmentVariable("ACCOUNT_WORLD_LIMIT");
    if (!string.IsNullOrWhiteSpace(worldLimitValue) && int.TryParse(worldLimitValue, out int worldLimit))
    {
      settings.WorldLimit = worldLimit;
    }

    return settings;
  }
}
