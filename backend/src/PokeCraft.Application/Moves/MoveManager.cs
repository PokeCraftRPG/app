using Logitar.EventSourcing;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;
using PokeCraft.Domain.Moves.Events;

namespace PokeCraft.Application.Moves;

public interface IMoveManager
{
  Task SaveAsync(Move move, CancellationToken cancellationToken = default);
}

internal class MoveManager : IMoveManager
{
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;
  private readonly IStorageService _storageService;

  public MoveManager(IMoveQuerier moveQuerier, IMoveRepository moveRepository, IStorageService storageService)
  {
    _moveQuerier = moveQuerier;
    _moveRepository = moveRepository;
    _storageService = storageService;
  }

  public async Task SaveAsync(Move move, CancellationToken cancellationToken)
  {
    UniqueName? uniqueName = null;
    foreach (IEvent change in move.Changes)
    {
      if (change is MoveCreated created)
      {
        uniqueName = created.UniqueName;
      }
      else if (change is MoveUpdated updated && updated.UniqueName is not null)
      {
        uniqueName = updated.UniqueName;
      }
    }

    if (uniqueName is not null)
    {
      MoveId? conflictId = await _moveQuerier.FindIdAsync(uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(move.Id))
      {
        throw new UniqueNameAlreadyUsedException(move, conflictId.Value);
      }
    }

    await _storageService.EnsureAvailableAsync(move, cancellationToken);

    await _moveRepository.SaveAsync(move, cancellationToken);

    await _storageService.UpdateAsync(move, cancellationToken);
  }
}
