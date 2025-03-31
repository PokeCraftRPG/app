using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Application.Worlds;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Queriers;

internal class WorldQuerier : IWorldQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<WorldEntity> _worlds;

  public WorldQuerier(IActorService actorService, PokemonContext context)
  {
    _actorService = actorService;
    _worlds = context.Worlds;
  }

  public async Task<int> CountAsync(UserId userId, CancellationToken cancellationToken)
  {
    Guid ownerId = userId.ToGuid();
    return await _worlds.AsNoTracking()
      .Where(x => x.OwnerId == ownerId)
      .CountAsync(cancellationToken);
  }

  public async Task<WorldId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    string? streamId = await _worlds.AsNoTracking()
      .Where(x => x.UniqueSlugNormalized == uniqueSlugNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new WorldId(streamId);
  }

  public async Task<UserId> FindOwnerIdAsync(WorldId worldId, CancellationToken cancellationToken)
  {
    string streamId = worldId.Value;

    Guid ownerId = await _worlds.AsNoTracking()
      .Where(x => x.StreamId == streamId)
      .Select(x => (Guid?)x.OwnerId)
      .SingleOrDefaultAsync(cancellationToken)
      ?? throw new InvalidOperationException($"The world entity 'StreamId={streamId}' could not be found.");

    return new UserId(ownerId);
  }

  public async Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken)
  {
    return await ReadAsync(world.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The world entity 'StreamId={world.Id}' could not be found.");
  }
  public async Task<WorldModel?> ReadAsync(WorldId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    WorldEntity? world = await _worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return world is null ? null : await MapAsync(world, cancellationToken);
  }
  public async Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return world is null ? null : await MapAsync(world, cancellationToken);
  }
  public async Task<WorldModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    WorldEntity? world = await _worlds.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return world is null ? null : await MapAsync(world, cancellationToken);
  }

  private async Task<WorldModel> MapAsync(WorldEntity world, CancellationToken cancellationToken)
  {
    return (await MapAsync([world], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<WorldModel>> MapAsync(IEnumerable<WorldEntity> worlds, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = worlds.SelectMany(world => world.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return worlds.Select(mapper.ToWorld).ToList().AsReadOnly();
  }
}
