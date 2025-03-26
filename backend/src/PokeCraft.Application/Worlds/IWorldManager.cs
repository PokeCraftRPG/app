using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds;

public interface IWorldManager
{
  Task SaveAsync(World world, CancellationToken cancellationToken = default);
}
