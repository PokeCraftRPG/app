using Logitar;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Speciez;
using PokeCraft.Domain.Speciez.Events;
using PokeCraft.Domain.Worlds;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.Handlers;

internal class SpeciesEvents : INotificationHandler<SpeciesCreated>,
  INotificationHandler<SpeciesDeleted>,
  INotificationHandler<SpeciesRegionalNumberChanged>,
  INotificationHandler<SpeciesUpdated>
{
  private readonly PokemonContext _context;
  private readonly ILogger<SpeciesEvents> _logger;

  public SpeciesEvents(PokemonContext context, ILogger<SpeciesEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public async Task Handle(SpeciesCreated @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is not null)
    {
      _logger.AlreadyExists(@event, species.Version);
      return;
    }

    WorldId worldId = new SpeciesId(@event.StreamId).WorldId;
    WorldEntity? world = await _context.Worlds.SingleOrDefaultAsync(x => x.StreamId == worldId.Value, cancellationToken);
    if (world is null)
    {
      _logger.WorldNotFound(@event, worldId);
      return;
    }

    species = new SpeciesEntity(world, @event);

    _context.Species.Add(species);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(SpeciesDeleted @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null)
    {
      _logger.NotFound(@event);
      return;
    }

    _context.Species.Remove(species);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(SpeciesRegionalNumberChanged @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species
      .Include(x => x.RegionalNumbers)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null)
    {
      _logger.NotFound(@event);
      return;
    }

    long expectedVersion = @event.Version - 1;
    if (expectedVersion != species.Version)
    {
      _logger.UnexpectedVersion(@event, species.Version);
      return;
    }

    RegionEntity? region = null;
    if (@event.Number is not null)
    {
      region = await _context.Regions.SingleOrDefaultAsync(x => x.StreamId == @event.RegionId.Value, cancellationToken);
      if (region is null)
      {
        _logger.LogWarning("[{Timestamp}] event.id='{Id}', event.type='{Type}', reason='{Reason}'; The region entity 'StreamId={StreamId}' was expected to exist but was not found.",
          DateTime.Now.ToISOString(), @event.Id, @event.GetType(), "RegionNotFound", @event.RegionId);
        return;
      }
    }
    species.SetRegionalNumber(region, @event);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }

  public async Task Handle(SpeciesUpdated @event, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _context.Species.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (species is null)
    {
      _logger.NotFound(@event);
      return;
    }

    long expectedVersion = @event.Version - 1;
    if (expectedVersion != species.Version)
    {
      _logger.UnexpectedVersion(@event, species.Version);
      return;
    }

    species.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);

    _logger.Success(@event);
  }
}
