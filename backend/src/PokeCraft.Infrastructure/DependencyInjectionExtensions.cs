using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application.Abilities;
using PokeCraft.Application.Accounts;
using PokeCraft.Application.Moves;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Speciez;
using PokeCraft.Application.Storages;
using PokeCraft.Application.Worlds;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Caching;
using PokeCraft.Infrastructure.Identity;
using PokeCraft.Infrastructure.Queriers;
using PokeCraft.Infrastructure.Repositories;
using PokeCraft.Infrastructure.Settings;

namespace PokeCraft.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftInfrastructure(this IServiceCollection services)
  {
    return services
      .AddIdentityServices()
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddMemoryCache()
      .AddQueriers()
      .AddRepositories()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<IActorService, ActorService>()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IEventBus, EventBus>();
  }

  private static IServiceCollection AddIdentityServices(this IServiceCollection services)
  {
    return services
      .AddSingleton<IApiKeyService, ApiKeyService>()
      .AddSingleton<IMessageService, MessageService>()
      .AddSingleton<IOneTimePasswordService, OneTimePasswordService>()
      .AddSingleton<IRealmService, RealmService>()
      .AddSingleton<ISessionService, SessionService>()
      .AddSingleton<ITokenService, TokenService>()
      .AddSingleton<IUserService, UserService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityQuerier, AbilityQuerier>()
      .AddScoped<IMoveQuerier, MoveQuerier>()
      .AddScoped<IRegionQuerier, RegionQuerier>()
      .AddScoped<ISpeciesQuerier, SpeciesQuerier>()
      .AddScoped<IWorldQuerier, WorldQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityRepository, AbilityRepository>()
      .AddScoped<IMoveRepository, MoveRepository>()
      .AddScoped<IRegionRepository, RegionRepository>()
      .AddScoped<ISpeciesRepository, SpeciesRepository>()
      .AddScoped<IStorageRepository, StorageRepository>()
      .AddScoped<IWorldRepository, WorldRepository>();
  }

  private static CachingSettings InitializeCachingSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return CachingSettings.Initialize(configuration);
  }
}
