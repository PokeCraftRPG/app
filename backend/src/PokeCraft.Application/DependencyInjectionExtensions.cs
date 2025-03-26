using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application.Worlds;

namespace PokeCraft.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftApplication(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcing()
      .AddManagers()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }

  private static IServiceCollection AddManagers(this IServiceCollection services)
  {
    return services.AddTransient<IWorldManager, WorldManager>();
  }
}
