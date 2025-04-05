using Logitar.EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Settings;
using PokeCraft.Application.Storages;

namespace PokeCraft.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftApplication(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcing()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton(InitializeAccountSettings)
      .AddTransient<IPermissionService, PermissionService>()
      .AddTransient<IStorageService, StorageService>();
  }

  private static AccountSettings InitializeAccountSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return AccountSettings.Initialize(configuration);
  }
}
