using Logitar.EventSourcing;
using PokeCraft.Application.Regions;
using PokeCraft.Domain.Regions;

namespace PokeCraft.Infrastructure.Repositories;

internal class RegionRepository : Repository, IRegionRepository
{
  public RegionRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Region?> LoadAsync(RegionId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Region>(id.StreamId, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Region>> LoadAsync(IEnumerable<RegionId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Region>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Region region, CancellationToken cancellationToken)
  {
    await base.SaveAsync(region, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Region> abilities, CancellationToken cancellationToken)
  {
    await base.SaveAsync(abilities, cancellationToken);
  }
}
