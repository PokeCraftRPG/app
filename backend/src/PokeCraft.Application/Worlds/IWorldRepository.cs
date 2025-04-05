using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds;

public interface IWorldRepository
{
  Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken = default);

  Task SaveAsync(World world, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<World> worlds, CancellationToken cancellationToken = default);
}
