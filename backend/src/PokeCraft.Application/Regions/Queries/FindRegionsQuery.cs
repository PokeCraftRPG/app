using MediatR;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Regions.Queries;

internal record FindRegionsQuery(IEnumerable<string> IdOrUniqueNames, string PropertyName) : IRequest<IReadOnlyDictionary<string, Region>>;

internal class FindRegionsQueryHandler : IRequestHandler<FindRegionsQuery, IReadOnlyDictionary<string, Region>>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public FindRegionsQueryHandler(IApplicationContext applicationContext, IRegionQuerier regionQuerier, IRegionRepository regionRepository)
  {
    _applicationContext = applicationContext;
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task<IReadOnlyDictionary<string, Region>> Handle(FindRegionsQuery query, CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<Guid, string> uniqueNameByIds = await _regionQuerier.GetUniqueNameByIdsAsync(cancellationToken);

    Dictionary<string, Guid> idByUniqueNames = new(capacity: uniqueNameByIds.Count);
    foreach (KeyValuePair<Guid, string> uniqueNameById in uniqueNameByIds)
    {
      idByUniqueNames[Normalize(uniqueNameById.Value)] = uniqueNameById.Key;
    }

    IEnumerable<string> idOrUniqueNames = query.IdOrUniqueNames;
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
      throw new RegionsNotFoundException(worldId, notFound, query.PropertyName);
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
      if (!Guid.TryParse(idOrUniqueName, out Guid id) || !regionsById.TryGetValue(id, out Region? region))
      {
        region = regionsByUniqueName[Normalize(idOrUniqueName)];
      }
      foundRegions[idOrUniqueName] = region;
    }
    return foundRegions.AsReadOnly();
  }

  private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
