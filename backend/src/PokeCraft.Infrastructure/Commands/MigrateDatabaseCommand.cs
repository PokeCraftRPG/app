using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace PokeCraft.Infrastructure.Commands;

public record MigrateDatabaseCommand : IRequest;

internal class MigrateDatabaseCommandHandler : IRequestHandler<MigrateDatabaseCommand>
{
  private readonly EventContext _event;
  private readonly PokemonContext _pokemon;

  public MigrateDatabaseCommandHandler(EventContext @event, PokemonContext pokemon)
  {
    _event = @event;
    _pokemon = pokemon;
  }

  public async Task Handle(MigrateDatabaseCommand _, CancellationToken cancellationToken)
  {
    await _event.Database.MigrateAsync(cancellationToken);
    await _pokemon.Database.MigrateAsync(cancellationToken);
  }
}
