using Logitar.EventSourcing;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Regions.Events;

namespace PokeCraft.Application.Regions;

public interface IRegionManager
{
  Task SaveAsync(Region region, CancellationToken cancellationToken = default);
}

internal class RegionManager : IRegionManager
{
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public RegionManager(IRegionQuerier regionQuerier, IRegionRepository regionRepository)
  {
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task SaveAsync(Region region, CancellationToken cancellationToken)
  {
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

    // TODO(fpion): ensure storage available

    await _regionRepository.SaveAsync(region, cancellationToken);

    // TODO(fpion): update storage
  }
}
