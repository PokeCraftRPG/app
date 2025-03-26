using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds;

public interface IWorldRepository
{
  Task<World?> LoadAsync(WorldId id, CancellationToken cancellationToken = default);
}
