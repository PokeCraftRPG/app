using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds;

public interface IWorldQuerier
{
  Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
}
