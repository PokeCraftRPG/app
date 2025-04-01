using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application.Abilities;
using PokeCraft.Application.Moves;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Speciez;
using PokeCraft.Application.Worlds;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Caching;
using PokeCraft.Infrastructure.Queriers;
using PokeCraft.Infrastructure.Repositories;

namespace PokeCraft.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddMemoryCache()
      .AddQueriers()
      .AddRepositories()
      .AddSingleton<IActorService, ActorService>()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IEventBus, EventBus>();
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
      .AddScoped<IWorldRepository, WorldRepository>();
  }
}
