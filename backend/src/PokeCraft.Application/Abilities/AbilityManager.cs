using Logitar.EventSourcing;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;
using PokeCraft.Domain.Abilities.Events;

namespace PokeCraft.Application.Abilities;

public interface IAbilityManager
{
  Task SaveAsync(Ability ability, CancellationToken cancellationToken = default);
}

internal class AbilityManager : IAbilityManager
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IStorageService _storageService;

  public AbilityManager(IAbilityQuerier abilityQuerier, IAbilityRepository abilityRepository, IStorageService storageService)
  {
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _storageService = storageService;
  }

  public async Task SaveAsync(Ability ability, CancellationToken cancellationToken)
  {
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
