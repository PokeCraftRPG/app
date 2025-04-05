using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Worlds.Events;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.Handlers;

internal class WorldEvents : INotificationHandler<WorldCreated>, INotificationHandler<WorldDeleted>, INotificationHandler<WorldUpdated>
{
  private readonly PokemonContext _context;
  private readonly ILogger<WorldEvents> _logger;

  public WorldEvents(PokemonContext context, ILogger<WorldEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(WorldCreated @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _context.Worlds.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (world is not null)
    {
      _logger.AlreadyExists(@event, world.Version);
      return;
    }

    world = new WorldEntity(@event);

    _context.Worlds.Add(world);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(WorldDeleted @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (world is null)
    {
      _logger.NotFound(@event);
      return;
    }

    _context.Worlds.Remove(world);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(WorldUpdated @event, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (world is null)
    {
      _logger.NotFound(@event);
      return;
    }

    long expectedVersion = @event.Version - 1;
    if (expectedVersion != world.Version)
    {
      _logger.UnexpectedVersion(@event, world.Version);
      return;
    }

    world.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }
}
