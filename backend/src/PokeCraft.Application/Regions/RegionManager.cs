﻿using Logitar.EventSourcing;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Regions.Events;

namespace PokeCraft.Application.Regions;

public interface IRegionManager
{
  Task<IReadOnlyDictionary<string, Region>> FindAsync(IEnumerable<string> idOrUniqueNames, CancellationToken cancellationToken = default);
  Task SaveAsync(Region region, CancellationToken cancellationToken = default);
}

internal class RegionManager : IRegionManager
{
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;
  private readonly IStorageService _storageService;

  public RegionManager(IRegionQuerier regionQuerier, IRegionRepository regionRepository, IStorageService storageService)
  {
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
    _storageService = storageService;
  }

  public async Task<IReadOnlyDictionary<string, Region>> FindAsync(IEnumerable<string> idOrUniqueNames, CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<Guid, string> uniqueNameByIds = await _regionQuerier.GetUniqueNameByIdsAsync(cancellationToken);

    throw new NotImplementedException(); // TODO(fpion): implement
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

    await _storageService.EnsureAvailableAsync(region, cancellationToken);

    await _regionRepository.SaveAsync(region, cancellationToken);

    await _storageService.UpdateAsync(region, cancellationToken);
  }
}
