using Microsoft.FeatureManagement;
using PokeCraft.Constants;
using PokeCraft.Extensions;

namespace PokeCraft;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddApplicationInsightsTelemetry();
    services.AddControllers();
    services.AddFeatureManagement();
    services.AddOpenApi();
    services.AddSwagger();

    IHealthChecksBuilder healthChecks = services.AddHealthChecks();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    ConfigureAsync((WebApplication)builder).Wait();
  }
  public virtual async Task ConfigureAsync(WebApplication application)
  {
    IFeatureManager featureManager = application.Services.GetRequiredService<IFeatureManager>();

    if (await featureManager.IsEnabledAsync(Features.UseSwaggerUI))
    {
      application.MapOpenApi();
      application.MapSwagger();
    }

    application.UseHttpsRedirection();

    application.MapControllers();
  }
}
