using MediatR;
using Microsoft.FeatureManagement;
using PokeCraft.Constants;
using PokeCraft.Infrastructure.Commands;

namespace PokeCraft;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    await startup.ConfigureAsync(application);

    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();
    if (await featureManager.IsEnabledAsync(Features.MigrateDatabase))
    {
      using IServiceScope scope = application.Services.CreateScope();
      IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      await mediator.Send(new MigrateDatabaseCommand());
    }

    application.Run();
  }
}
