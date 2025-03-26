using Logitar.EventSourcing;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;
using PokeCraft.Domain.Worlds.Events;

namespace PokeCraft.Application.Worlds;

public interface IWorldManager
{
  Task SaveAsync(World world, CancellationToken cancellationToken = default);
}

internal class WorldManager : IWorldManager
{
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public WorldManager(IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task SaveAsync(World world, CancellationToken cancellationToken)
  {
    Slug? uniqueSlug = null;
    foreach (IEvent change in world.Changes)
    {
      if (change is WorldCreated created)
      {
        uniqueSlug = created.UniqueSlug;
      }
      else if (change is WorldUpdated updated && updated.UniqueSlug is not null)
      {
        uniqueSlug = updated.UniqueSlug;
      }
    }

    if (uniqueSlug is not null)
    {
      WorldId? conflictId = await _worldQuerier.FindIdAsync(uniqueSlug, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(world.Id))
      {
        throw new UniqueSlugAlreadyUsedException(world, conflictId.Value);
      }
    }

    // TODO(fpion): ensure storage available

    await _worldRepository.SaveAsync(world, cancellationToken);

    // TODO(fpion): update storage
  }
}
