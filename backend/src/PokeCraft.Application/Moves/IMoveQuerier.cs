using PokeCraft.Application.Moves.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves;

public interface IMoveQuerier
{
  Task<MoveId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<MoveModel> ReadAsync(Move move, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(MoveId id, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<MoveModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
