using Logitar.EventSourcing;
using PokeCraft.Application.Worlds;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Infrastructure.Repositories;

internal class WorldRepository : Repository, IWorldRepository
{
  public WorldRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<World>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(World world, CancellationToken cancellationToken)
  {
    await base.SaveAsync(world, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<World> worlds, CancellationToken cancellationToken)
  {
    await base.SaveAsync(worlds, cancellationToken);
  }
}
