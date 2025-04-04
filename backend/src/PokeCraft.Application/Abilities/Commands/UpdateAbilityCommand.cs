﻿using FluentValidation;
using MediatR;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Application.Abilities.Validators;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;

namespace PokeCraft.Application.Abilities.Commands;

public record UpdateAbilityCommand(Guid Id, UpdateAbilityPayload Payload) : IRequest<AbilityModel?>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateAbilityCommandHandler : IRequestHandler<UpdateAbilityCommand, AbilityModel?>
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPermissionService _permissionService;

  public UpdateAbilityCommandHandler(
    IAbilityQuerier abilityQuerier,
    IAbilityRepository abilityRepository,
    IApplicationContext applicationContext,
    IMediator mediator,
    IPermissionService permissionService)
  {
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _applicationContext = applicationContext;
    _mediator = mediator;
    _permissionService = permissionService;
  }

  public async Task<AbilityModel?> Handle(UpdateAbilityCommand command, CancellationToken cancellationToken)
  {
    UpdateAbilityPayload payload = command.Payload;
    new UpdateAbilityValidator().ValidateAndThrow(payload);

    AbilityId id = new(_applicationContext.WorldId, command.Id);
    Ability? ability = await _abilityRepository.LoadAsync(id, cancellationToken);
    if (ability is null)
    {
      return null;
    }
    await _permissionService.EnsureCanUpdateAsync(ability, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      ability.UniqueName = new UniqueName(payload.UniqueName);
    }
    if (payload.DisplayName is not null)
    {
      ability.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      ability.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Link is not null)
    {
      ability.Link = Url.TryCreate(payload.Link.Value);
    }
    if (payload.Notes is not null)
    {
      ability.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    ability.Update(_applicationContext.UserId);
    await _mediator.Send(new SaveAbilityCommand(ability), cancellationToken);

    return await _abilityQuerier.ReadAsync(ability, cancellationToken);
  }
}
