using Logitar.EventSourcing;
using MediatR;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Regions.Events;

namespace PokeCraft.Application.Regions.Commands;

internal record SaveRegionCommand(Region Region) : IRequest;

internal class SaveRegionCommandHandler : IRequestHandler<SaveRegionCommand>
{
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;
  private readonly IStorageService _storageService;

  public SaveRegionCommandHandler(IRegionQuerier regionQuerier, IRegionRepository regionRepository, IStorageService storageService)
  {
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveRegionCommand command, CancellationToken cancellationToken)
  {
    Region region = command.Region;

    UniqueName? uniqueName = null;
    foreach (IEvent change in region.Changes)
    {
      if (change is RegionCreated created)
      {
        uniqueName = created.UniqueName;
      }
      else if (change is RegionUpdated updated && updated.UniqueName is not null)
      {
        uniqueName = updated.UniqueName;
      }
    }

    if (uniqueName is not null)
    {
      RegionId? conflictId = await _regionQuerier.FindIdAsync(uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(region.Id))
      {
        throw new UniqueNameAlreadyUsedException(region, conflictId.Value);
      }
    }

    await _storageService.EnsureAvailableAsync(region, cancellationToken);

    await _regionRepository.SaveAsync(region, cancellationToken);

    await _storageService.UpdateAsync(region, cancellationToken);
  }
}
