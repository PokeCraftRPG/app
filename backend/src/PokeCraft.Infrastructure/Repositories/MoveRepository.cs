using Logitar.EventSourcing;
using PokeCraft.Application.Moves;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Infrastructure.Repositories;

internal class MoveRepository : Repository, IMoveRepository
{
  public MoveRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Move?> LoadAsync(MoveId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Move>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Move move, CancellationToken cancellationToken)
  {
    await base.SaveAsync(move, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Move> abilities, CancellationToken cancellationToken)
  {
    await base.SaveAsync(abilities, cancellationToken);
  }
}
