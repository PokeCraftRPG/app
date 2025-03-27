using FluentValidation;
using MediatR;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Application.Abilities.Validators;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;

namespace PokeCraft.Application.Abilities.Commands;

public record CreateOrReplaceAbilityResult(AbilityModel Ability, bool Created);

public record CreateOrReplaceAbilityCommand(Guid? Id, CreateOrReplaceAbilityPayload Payload) : IRequest<CreateOrReplaceAbilityResult>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceAbilityCommandHandler : IRequestHandler<CreateOrReplaceAbilityCommand, CreateOrReplaceAbilityResult>
{
  private readonly IAbilityManager _abilityManager;
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceAbilityCommandHandler(
    IAbilityManager abilityManager,
    IAbilityQuerier abilityQuerier,
    IAbilityRepository abilityRepository,
    IApplicationContext applicationContext,
    IPermissionService permissionService)
  {
    _abilityManager = abilityManager;
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _applicationContext = applicationContext;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceAbilityResult> Handle(CreateOrReplaceAbilityCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceAbilityPayload payload = command.Payload;
    new CreateOrReplaceAbilityValidator().ValidateAndThrow(payload);

    AbilityId id = AbilityId.NewId(_applicationContext.WorldId);
    Ability? ability = null;
    if (command.Id.HasValue)
    {
      id = new(id.WorldId, command.Id.Value);
      ability = await _abilityRepository.LoadAsync(id, cancellationToken);
    }

    UserId userId = _applicationContext.UserId;
    UniqueName uniqueName = new(payload.UniqueName);

    bool created = false;
    if (ability is null)
    {
      await _permissionService.EnsureCanCreateAsync(ResourceType.Ability, cancellationToken);

      ability = new(uniqueName, userId, id);
      created = true;
    }
    else
    {
      await _permissionService.EnsureCanUpdateAsync(ability, cancellationToken);

      ability.UniqueName = uniqueName;
    }

    ability.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    ability.Description = Description.TryCreate(payload.Description);

    ability.Link = Url.TryCreate(payload.Link);
    ability.Notes = Notes.TryCreate(payload.Notes);

    ability.Update(userId);
    await _abilityManager.SaveAsync(ability, cancellationToken);

    AbilityModel model = await _abilityQuerier.ReadAsync(ability, cancellationToken);
    return new CreateOrReplaceAbilityResult(model, created);
  }
}
