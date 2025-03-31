﻿using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application.Abilities;
using PokeCraft.Application.Moves;
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
      .AddMemoryCache()
      .AddQueriers()
      .AddRepositories()
      .AddSingleton<IActorService, ActorService>()
      .AddSingleton<ICacheService, CacheService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityQuerier, AbilityQuerier>()
      .AddScoped<IMoveQuerier, MoveQuerier>()
      .AddScoped<IWorldQuerier, WorldQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IAbilityRepository, AbilityRepository>()
      .AddScoped<IMoveRepository, MoveRepository>()
      .AddScoped<IWorldRepository, WorldRepository>();
  }
}
