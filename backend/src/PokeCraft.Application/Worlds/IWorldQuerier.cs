using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds;

public interface IWorldQuerier
{
  Task<int> CountAsync(UserId ownerId, CancellationToken cancellationToken = default);

  Task<WorldId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken = default);

  Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(WorldId id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<WorldModel?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
}
