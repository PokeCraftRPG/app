using PokeCraft.Domain.Abilities;

namespace PokeCraft.Application.Abilities;

public interface IAbilityRepository
{
  Task<Ability?> LoadAsync(AbilityId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Ability ability, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Ability> abilitys, CancellationToken cancellationToken = default);
}
