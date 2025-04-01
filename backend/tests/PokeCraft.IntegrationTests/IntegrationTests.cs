using Bogus;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Actors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Application;
using PokeCraft.Infrastructure;
using PokeCraft.Infrastructure.Commands;
using PokeCraft.Infrastructure.SqlServer;
using System.Text;
using PokemonDb = PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;

  protected Faker Faker { get; } = new();

  protected IServiceProvider ServiceProvider { get; }
  protected IMediator Mediator { get; }
  protected EventContext EventContext { get; }
  protected PokemonContext PokemonContext { get; }

  protected ActorModel Actor { get; }

  protected IntegrationTests()
  {
    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(configuration);
    services.AddPokeCraftApplication();
    services.AddPokeCraftInfrastructure();

    string? connectionString;
    _databaseProvider = GetDatabaseProvider(configuration);
    switch (_databaseProvider)
    {
      case DatabaseProvider.SqlServer:
        connectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_Pokemon");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
          connectionString = configuration.GetConnectionString(DatabaseProvider.SqlServer.ToString());
        }
        if (string.IsNullOrWhiteSpace(connectionString))
        {
          throw new ArgumentException($"The connection string for the database provider '{DatabaseProvider.SqlServer}' could not be found.", nameof(configuration));
        }
        connectionString = connectionString.Replace("{Database}", GetType().Name);
        services.AddPokeCraftInfrastructureSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    Actor = new ActorModel
    {
      Id = Guid.NewGuid(),
      Type = ActorType.User,
      DisplayName = Faker.Person.FullName,
      EmailAddress = Faker.Person.Email,
      PictureUrl = Faker.Person.Avatar
    };
    TestApplicationContext applicationContext = new(Actor);
    services.AddSingleton<IApplicationContext>(applicationContext);

    ServiceProvider = services.BuildServiceProvider();
    Mediator = ServiceProvider.GetRequiredService<IMediator>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    PokemonContext = ServiceProvider.GetRequiredService<PokemonContext>();
  }

  public virtual async Task InitializeAsync()
  {
    await Mediator.Send(new MigrateDatabaseCommand());

    StringBuilder statement = new();
    TableId[] tables =
    [
      PokemonDb.Species.Table,
      PokemonDb.Regions.Table,
      PokemonDb.Moves.Table,
      PokemonDb.Abilities.Table,
      PokemonDb.Worlds.Table,
      EventDb.Streams.Table
    ];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      statement.AppendLine(command.Text);
    }
    await PokemonContext.Database.ExecuteSqlRawAsync(statement.ToString());
  }
  private IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.SqlServer => new SqlServerDeleteBuilder(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public virtual Task DisposeAsync() => Task.CompletedTask;

  private static DatabaseProvider GetDatabaseProvider(IConfiguration configuration)
  {
    string? databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER");
    return !string.IsNullOrWhiteSpace(databaseProvider)
      ? Enum.Parse<DatabaseProvider>(databaseProvider)
      : (configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.SqlServer);
  }
}
