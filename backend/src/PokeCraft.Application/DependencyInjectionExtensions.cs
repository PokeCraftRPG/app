using Logitar.EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Settings;
using PokeCraft.Application.Speciez;
using PokeCraft.Application.Storages;
using PokeCraft.Application.Worlds;

namespace PokeCraft.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftApplication(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcing()
      .AddManagers()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton(InitializeAccountSettings)
      .AddTransient<IPermissionService, PermissionService>()
      .AddTransient<IStorageService, StorageService>();
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IRegionManager, RegionManager>()
      .AddTransient<ISpeciesManager, SpeciesManager>()
      .AddTransient<IWorldManager, WorldManager>();
  }

  private static AccountSettings InitializeAccountSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return AccountSettings.Initialize(configuration);
  }
}
