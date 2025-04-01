using Microsoft.Extensions.Configuration;

namespace PokeCraft.Infrastructure.Settings;

internal record CachingSettings
{
  public TimeSpan ActorLifetime { get; set; } = TimeSpan.FromMinutes(15);

  public static CachingSettings Initialize(IConfiguration configuration)
  {
    CachingSettings settings = configuration.GetSection("Caching").Get<CachingSettings>() ?? new();

    string? actorLifetimeValue = Environment.GetEnvironmentVariable("CACHING_ACTOR_LIFETIME");
    if (!string.IsNullOrWhiteSpace(actorLifetimeValue) && TimeSpan.TryParse(actorLifetimeValue, out TimeSpan actorLifetime))
    {
      settings.ActorLifetime = actorLifetime;
    }

    return settings;
  }
}
