using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PokeCraft.Infrastructure.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftInfrastructureSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_Pokemon");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      connectionString = configuration.GetConnectionString("SqlServer");
    }
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The connection string for the database provider '{DatabaseProvider.SqlServer}' could not be found.", nameof(configuration));
    }
    return services.AddPokeCraftInfrastructureSqlServer(connectionString);
  }
  public static IServiceCollection AddPokeCraftInfrastructureSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddDbContext<PokemonContext>(options => options.UseSqlServer(connectionString, options => options.MigrationsAssembly("PokeCraft.Infrastructure.SqlServer")));
  }
}
