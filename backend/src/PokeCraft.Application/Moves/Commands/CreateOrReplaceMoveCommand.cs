using FluentValidation;
using MediatR;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Application.Moves.Validators;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves.Commands;

public record CreateOrReplaceMoveResult(MoveModel Move, bool Created);

public record CreateOrReplaceMoveCommand(Guid? Id, CreateOrReplaceMovePayload Payload) : IRequest<CreateOrReplaceMoveResult>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="StatusMoveCannotHavePowerException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceMoveCommandHandler : IRequestHandler<CreateOrReplaceMoveCommand, CreateOrReplaceMoveResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMoveManager _moveManager;
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceMoveCommandHandler(
    IApplicationContext applicationContext,
    IMoveManager moveManager,
    IMoveQuerier moveQuerier,
    IMoveRepository moveRepository,
    IPermissionService permissionService)
  {
    _applicationContext = applicationContext;
    _moveManager = moveManager;
    _moveQuerier = moveQuerier;
    _moveRepository = moveRepository;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceMoveResult> Handle(CreateOrReplaceMoveCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceMovePayload payload = command.Payload;
    new CreateOrReplaceMoveValidator().ValidateAndThrow(payload);

    MoveId id = MoveId.NewId(_applicationContext.WorldId);
    Move? move = null;
    if (command.Id.HasValue)
    {
      id = new(id.WorldId, command.Id.Value);
      move = await _moveRepository.LoadAsync(id, cancellationToken);
    }

    UserId userId = _applicationContext.UserId;
    UniqueName uniqueName = new(payload.UniqueName);
    PowerPoints powerPoints = new(payload.PowerPoints);

    bool created = false;
    if (move is null)
    {
      await _permissionService.EnsureCanCreateAsync(ResourceType.Move, cancellationToken);

      move = new(payload.Type, payload.Category, uniqueName, powerPoints, userId, id);
      created = true;
    }
    else
    {
      await _permissionService.EnsureCanUpdateAsync(move, cancellationToken);

      move.UniqueName = uniqueName;
      move.PowerPoints = powerPoints;
    }

    move.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    move.Description = Description.TryCreate(payload.Description);

    move.Accuracy = Accuracy.TryCreate(payload.Accuracy);
    move.Power = Power.TryCreate(payload.Power);

    move.InflictedStatus = InflictedStatus.TryCreate(payload.InflictedStatus);
    SetStatisticChanges(move, payload);
    move.SetVolatileConditions(payload.VolatileConditions.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new VolatileCondition(x)));

    move.Link = Url.TryCreate(payload.Link);
    move.Notes = Notes.TryCreate(payload.Notes);

    move.Update(userId);
    await _moveManager.SaveAsync(move, cancellationToken);

    MoveModel model = await _moveQuerier.ReadAsync(move, cancellationToken);
    return new CreateOrReplaceMoveResult(model, created);
  }

  private static void SetStatisticChanges(Move move, CreateOrReplaceMovePayload payload)
  {
    HashSet<PokemonStatistic> statisticsToRemove = move.StatisticChanges.Keys.ToHashSet();
    foreach (StatisticChangeModel change in payload.StatisticChanges)
    {
      move.SetStatisticChange(change.Statistic, change.Stages);
      statisticsToRemove.Remove(change.Statistic);
    }
    foreach (PokemonStatistic statistic in statisticsToRemove)
    {
      move.SetStatisticChange(statistic, stages: 0);
    }
  }
}
