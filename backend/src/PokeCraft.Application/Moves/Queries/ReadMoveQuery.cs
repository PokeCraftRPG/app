using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Application.Permissions;
using PokeCraft.Domain;

namespace PokeCraft.Application.Moves.Queries;

public record ReadMoveQuery(Guid? Id, string? UniqueName) : IRequest<MoveModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadMoveQueryHandler : IRequestHandler<ReadMoveQuery, MoveModel?>
{
  private readonly IMoveQuerier _moveQuerier;
  private readonly IPermissionService _permissionService;

  public ReadMoveQueryHandler(IMoveQuerier moveQuerier, IPermissionService permissionService)
  {
    _moveQuerier = moveQuerier;
    _permissionService = permissionService;
  }

  public async Task<MoveModel?> Handle(ReadMoveQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanViewAsync(ResourceType.Move, cancellationToken);

    Dictionary<Guid, MoveModel> moves = new(capacity: 2);

    if (query.Id.HasValue)
    {
      MoveModel? move = await _moveQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (move is not null)
      {
        moves[move.Id] = move;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      MoveModel? move = await _moveQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (move is not null)
      {
        moves[move.Id] = move;
      }
    }

    if (moves.Count > 1)
    {
      throw TooManyResultsException<MoveModel>.ExpectedSingle(moves.Count);
    }

    return moves.Values.SingleOrDefault();
  }
}
