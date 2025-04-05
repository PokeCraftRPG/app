using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Regions.Events;
using PokeCraft.Domain.Worlds;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.Handlers;

internal class RegionEvents : INotificationHandler<RegionCreated>, INotificationHandler<RegionDeleted>, INotificationHandler<RegionUpdated>
{
  private readonly PokemonContext _context;
  private readonly ILogger<RegionEvents> _logger;

  public RegionEvents(PokemonContext context, ILogger<RegionEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(RegionCreated @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is not null)
    {
      _logger.AlreadyExists(@event, region.Version);
      return;
    }

    WorldId worldId = new RegionId(@event.StreamId).WorldId;
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.StreamId == worldId.Value, cancellationToken);
    if (world is null)
    {
      _logger.WorldNotFound(@event, worldId);
      return;
    }

    region = new RegionEntity(world, @event);

    _context.Regions.Add(region);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(RegionDeleted @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is null)
    {
      _logger.NotFound(@event);
      return;
    }

    _context.Regions.Remove(region);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(RegionUpdated @event, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _context.Regions.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (region is null)
    {
      _logger.NotFound(@event);
      return;
    }

    long expectedVersion = @event.Version - 1;
    if (expectedVersion != region.Version)
    {
      _logger.UnexpectedVersion(@event, region.Version);
      return;
    }

    region.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }
}
