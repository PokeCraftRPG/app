using Logitar.EventSourcing;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Regions.Events;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Regions;

public interface IRegionManager
{
  Task<IReadOnlyDictionary<string, Region>> FindAsync(IEnumerable<string> idOrUniqueNames, CancellationToken cancellationToken = default);
  Task SaveAsync(Region region, CancellationToken cancellationToken = default);
}

internal class RegionManager : IRegionManager
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;
  private readonly IStorageService _storageService;

  public RegionManager(
    IApplicationContext applicationContext,
    IRegionQuerier regionQuerier,
    IRegionRepository regionRepository,
    IStorageService storageService)
  {
    _applicationContext = applicationContext;
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
    _storageService = storageService;
  }

  public async Task<IReadOnlyDictionary<string, Region>> FindAsync(IEnumerable<string> idOrUniqueNames, CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<Guid, string> uniqueNameByIds = await _regionQuerier.GetUniqueNameByIdsAsync(cancellationToken);

    Dictionary<string, Guid> idByUniqueNames = new(capacity: uniqueNameByIds.Count);
    foreach (KeyValuePair<Guid, string> uniqueNameById in uniqueNameByIds)
    {
      idByUniqueNames[Normalize(uniqueNameById.Value)] = uniqueNameById.Key;
    }

    int capacity = idOrUniqueNames.Count();
    HashSet<RegionId> ids = new(capacity);
    HashSet<string> notFound = new(capacity);
    WorldId worldId = _applicationContext.WorldId;
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      if ((Guid.TryParse(idOrUniqueName, out Guid entityId) && uniqueNameByIds.ContainsKey(entityId))
        || idByUniqueNames.TryGetValue(Normalize(idOrUniqueName), out entityId))
      {
        ids.Add(new RegionId(worldId, entityId));
      }
      else
      {
        notFound.Add(idOrUniqueName);
      }
    }

    if (notFound.Count > 0)
    {
      // TODO(fpion): implement
    }

    IReadOnlyCollection<Region> regions = await _regionRepository.LoadAsync(ids, cancellationToken);
    Dictionary<Guid, Region> regionsById = new(capacity: regions.Count);
    Dictionary<string, Region> regionsByUniqueName = new(capacity: regions.Count);
    foreach (Region region in regions)
    {
      regionsById[region.EntityId] = region;
      regionsByUniqueName[Normalize(region.UniqueName.Value)] = region;
    }

    Dictionary<string, Region> foundRegions = new(capacity);
    foreach (string idOrUniqueName in idOrUniqueNames)
    {
      // TODO(fpion): implement
    }
    return foundRegions.AsReadOnly();
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

  private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
