using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Moves;
using PokeCraft.Domain.Moves.Events;
using PokeCraft.Domain.Worlds;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.Handlers;

internal class MoveEvents : INotificationHandler<MoveCreated>, INotificationHandler<MoveDeleted>, INotificationHandler<MoveUpdated>
{
  private readonly PokemonContext _context;
  private readonly ILogger<MoveEvents> _logger;

  public MoveEvents(PokemonContext context, ILogger<MoveEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(MoveCreated @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is not null)
    {
      _logger.AlreadyExists(@event, move.Version);
      return;
    }

    WorldId worldId = new MoveId(@event.StreamId).WorldId;
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.StreamId == worldId.Value, cancellationToken);
    if (world is null)
    {
      _logger.WorldNotFound(@event, worldId);
      return;
    }

    move = new MoveEntity(world, @event);

    _context.Moves.Add(move);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(MoveDeleted @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is null)
    {
      _logger.NotFound(@event);
      return;
    }

    _context.Moves.Remove(move);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(MoveUpdated @event, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _context.Moves.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (move is null)
    {
      _logger.NotFound(@event);
      return;
    }

    long expectedVersion = @event.Version - 1;
    if (expectedVersion != move.Version)
    {
      _logger.UnexpectedVersion(@event, move.Version);
      return;
    }

    move.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }
}
