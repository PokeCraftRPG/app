using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.FeatureManagement;
using PokeCraft.Application;
using PokeCraft.Authentication;
using PokeCraft.Constants;
using PokeCraft.Extensions;
using PokeCraft.Infrastructure;
using PokeCraft.Infrastructure.SqlServer;
using PokeCraft.Middlewares;
using PokeCraft.Settings;

namespace PokeCraft;

internal class Startup : StartupBase
{
  private readonly string[] _authenticationSchemes;
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _authenticationSchemes = Schemes.GetEnabled(configuration);
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    OpenAuthenticationSettings openAuthenticationSettings = OpenAuthenticationSettings.Initialize(_configuration);
    services.AddSingleton(openAuthenticationSettings);
    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (_authenticationSchemes.Contains(Schemes.Basic))
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }
    services.AddSingleton<IOpenAuthenticationService, OpenAuthenticationService>();

    services.AddAuthorizationBuilder().SetDefaultPolicy(new AuthorizationPolicyBuilder(_authenticationSchemes).RequireAuthenticatedUser().Build());

    CookiesSettings cookiesSettings = CookiesSettings.Initialize(_configuration);
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    services.AddSingleton(CorsSettings.Initialize(_configuration));
    services.AddCors();

    services.AddApplicationInsightsTelemetry();
    services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    services.AddExceptionHandler<ExceptionHandler>();
    services.AddFeatureManagement();
    services.AddProblemDetails();
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

    services.AddLogitarPortalClient(InitializePortalSettings());
  }
  private DatabaseProvider GetDatabaseProvider()
  {
    string? value = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
    if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, out DatabaseProvider databaseProvider))
    {
      return databaseProvider;
    }
    return _configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.SqlServer;
  }
  private PortalSettings InitializePortalSettings()
  {
    PortalSettings settings = _configuration.GetSection(PortalSettings.SectionKey).Get<PortalSettings>() ?? new();

    settings.ApiKey = EnvironmentHelper.TryGetVariable("PORTAL_API_KEY", settings.ApiKey);
    settings.BaseUrl = EnvironmentHelper.TryGetVariable("PORTAL_BASE_URL", settings.BaseUrl);
    settings.Realm = EnvironmentHelper.TryGetVariable("PORTAL_REALM", settings.Realm);

    string? username = EnvironmentHelper.TryGetVariable("PORTAL_USERNAME", settings.Basic?.Username);
    string? password = EnvironmentHelper.TryGetVariable("PORTAL_PASSWORD", settings.Basic?.Password);
    if (username is not null && password is not null)
    {
      settings.Basic = new BasicCredentials(username, password);
    }

    return settings;
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
    application.UseCors();
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<RenewSession>();
    application.UseAuthentication();
    application.UseAuthorization();
    application.UseMiddleware<ResolveWorld>();

    application.MapControllers();
    application.MapHealthChecks("/health");
  }
}
