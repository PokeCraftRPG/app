using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.FeatureManagement;
using PokeCraft.Application;
using PokeCraft.Constants;
using PokeCraft.Extensions;
using PokeCraft.Infrastructure;
using PokeCraft.Infrastructure.SqlServer;

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
    services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    services.AddFeatureManagement();
    services.AddOpenApi();
    services.AddSwagger();

    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddPokeCraftApplication();
    services.AddPokeCraftInfrastructure();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();

    DatabaseProvider databaseProvider = GetDatabaseProvider();
    switch (databaseProvider)
    {
      case DatabaseProvider.SqlServer:
        services.AddPokeCraftInfrastructureSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<PokemonContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }
  private DatabaseProvider GetDatabaseProvider()
  {
    string? databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
    return !string.IsNullOrWhiteSpace(databaseProvider)
      ? Enum.Parse<DatabaseProvider>(databaseProvider)
      : (_configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.SqlServer);
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
