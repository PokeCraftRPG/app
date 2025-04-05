using FluentValidation;
using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Application.Moves.Validators;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves.Commands;

public record UpdateMoveCommand(Guid Id, UpdateMovePayload Payload) : IRequest<MoveModel?>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="StatusMoveCannotHavePowerException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateMoveCommandHandler : IRequestHandler<UpdateMoveCommand, MoveModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;
  private readonly IPermissionService _permissionService;

  public UpdateMoveCommandHandler(
    IApplicationContext applicationContext,
    IMediator mediator,
    IMoveQuerier moveQuerier,
    IMoveRepository moveRepository,
    IPermissionService permissionService)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _moveQuerier = moveQuerier;
    _moveRepository = moveRepository;
    _permissionService = permissionService;
  }

  public async Task<MoveModel?> Handle(UpdateMoveCommand command, CancellationToken cancellationToken)
  {
    UpdateMovePayload payload = command.Payload;
    new UpdateMoveValidator().ValidateAndThrow(payload);

    MoveId id = new(_applicationContext.WorldId, command.Id);
    Move? move = await _moveRepository.LoadAsync(id, cancellationToken);
    if (move is null)
    {
      return null;
    }
    await _permissionService.EnsureCanUpdateAsync(move, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      move.UniqueName = new UniqueName(payload.UniqueName);
    }
    if (payload.DisplayName is not null)
    {
      move.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      move.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Accuracy is not null)
    {
      move.Accuracy = Accuracy.TryCreate(payload.Accuracy.Value);
    }
    if (payload.Power is not null)
    {
      move.Power = Power.TryCreate(payload.Power.Value);
    }
    if (payload.PowerPoints.HasValue)
    {
      move.PowerPoints = new PowerPoints(payload.PowerPoints.Value);
    }

    if (payload.InflictedStatus is not null)
    {
      move.InflictedStatus = InflictedStatus.TryCreate(payload.InflictedStatus.Value);
    }
    foreach (StatisticChangeModel change in payload.StatisticChanges)
    {
      move.SetStatisticChange(change.Statistic, change.Stages);
    }
    SetVolatileConditions(move, payload);

    if (payload.Link is not null)
    {
      move.Link = Url.TryCreate(payload.Link.Value);
    }
    if (payload.Notes is not null)
    {
      move.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    move.Update(_applicationContext.UserId);
    await _mediator.Send(new SaveMoveCommand(move), cancellationToken);

    return await _moveQuerier.ReadAsync(move, cancellationToken);
  }

  private static void SetVolatileConditions(Move move, UpdateMovePayload payload)
  {
    HashSet<VolatileCondition> volatileConditions = [.. move.VolatileConditions];
    foreach (VolatileConditionAction value in payload.VolatileConditions)
    {
      VolatileCondition volatileCondition = new(value.Value);
      switch (value.Action)
      {
        case CollectionAction.Add:
          volatileConditions.Add(volatileCondition);
          break;
        case CollectionAction.Remove:
          volatileConditions.Remove(volatileCondition);
          break;
      }
    }
    move.SetVolatileConditions(volatileConditions);
  }
}
