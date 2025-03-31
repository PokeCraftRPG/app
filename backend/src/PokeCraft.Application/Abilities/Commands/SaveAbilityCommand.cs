using Logitar.EventSourcing;
using MediatR;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;
using PokeCraft.Domain.Abilities.Events;

namespace PokeCraft.Application.Abilities.Commands;

internal record SaveAbilityCommand(Ability Ability) : IRequest;

internal class SaveAbilityCommandHandler : IRequestHandler<SaveAbilityCommand>
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IStorageService _storageService;

  public SaveAbilityCommandHandler(IAbilityQuerier abilityQuerier, IAbilityRepository abilityRepository, IStorageService storageService)
  {
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveAbilityCommand command, CancellationToken cancellationToken)
  {
    Ability ability = command.Ability;

    UniqueName? uniqueName = null;
    foreach (IEvent change in ability.Changes)
    {
      if (change is AbilityCreated created)
      {
        uniqueName = created.UniqueName;
      }
      else if (change is AbilityUpdated updated && updated.UniqueName is not null)
      {
        uniqueName = updated.UniqueName;
      }
    }

    if (uniqueName is not null)
    {
      AbilityId? conflictId = await _abilityQuerier.FindIdAsync(uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(ability.Id))
      {
        throw new UniqueNameAlreadyUsedException(ability, conflictId.Value);
      }
    }

    await _storageService.EnsureAvailableAsync(ability, cancellationToken);

    await _abilityRepository.SaveAsync(ability, cancellationToken);

    await _storageService.UpdateAsync(ability, cancellationToken);
  }
}
