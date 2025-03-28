using Logitar.EventSourcing;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Storages;
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
  private readonly IStorageService _storageService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public WorldManager(IStorageService storageService, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _storageService = storageService;
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

    Resource resource = Resource.From(world);
    await _storageService.EnsureAvailableAsync(resource, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    await _storageService.UpdateAsync(resource, cancellationToken);
  }
}
