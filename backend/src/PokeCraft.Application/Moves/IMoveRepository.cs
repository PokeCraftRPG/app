using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves;

public interface IMoveRepository
{
  Task<Move?> LoadAsync(MoveId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Move move, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Move> moves, CancellationToken cancellationToken = default);
}
