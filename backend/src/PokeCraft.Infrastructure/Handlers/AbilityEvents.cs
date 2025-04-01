using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Abilities;
using PokeCraft.Domain.Abilities.Events;
using PokeCraft.Domain.Worlds;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.Handlers;

internal class AbilityEvents : INotificationHandler<AbilityCreated>, INotificationHandler<AbilityDeleted>, INotificationHandler<AbilityUpdated>
{
  private readonly PokemonContext _context;
  private readonly ILogger<AbilityEvents> _logger;

  public AbilityEvents(PokemonContext context, ILogger<AbilityEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(AbilityCreated @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is not null)
    {
      _logger.AlreadyExists(@event, ability.Version);
      return;
    }

    WorldId worldId = new AbilityId(@event.StreamId).WorldId;
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.StreamId == worldId.Value, cancellationToken);
    if (world is null)
    {
      _logger.WorldNotFound(@event, worldId);
      return;
    }

    ability = new AbilityEntity(world, @event);

    _context.Abilities.Add(ability);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(AbilityDeleted @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is null)
    {
      _logger.NotFound(@event);
      return;
    }

    _context.Abilities.Remove(ability);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(AbilityUpdated @event, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _context.Abilities.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ability is null)
    {
      _logger.NotFound(@event);
      return;
    }

    long expectedVersion = @event.Version - 1;
    if (expectedVersion != ability.Version)
    {
      _logger.UnexpectedVersion(@event, ability.Version);
      return;
    }

    ability.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }
}
