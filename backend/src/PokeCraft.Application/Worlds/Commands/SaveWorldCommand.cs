using Logitar.EventSourcing;
using MediatR;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;
using PokeCraft.Domain.Worlds.Events;

namespace PokeCraft.Application.Worlds.Commands;

internal record SaveWorldCommand(World World) : IRequest;

internal class SaveWorldCommandHandler : IRequestHandler<SaveWorldCommand>
{
  private readonly IStorageService _storageService;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public SaveWorldCommandHandler(IStorageService storageService, IWorldQuerier worldQuerier, IWorldRepository worldRepository)
  {
    _storageService = storageService;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task Handle(SaveWorldCommand command, CancellationToken cancellationToken)
  {
    World world = command.World;

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

    await _storageService.EnsureAvailableAsync(world, cancellationToken);

    await _worldRepository.SaveAsync(world, cancellationToken);

    await _storageService.UpdateAsync(world, cancellationToken);
  }
}
