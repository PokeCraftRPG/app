using PokeCraft.Application.Abilities.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;

namespace PokeCraft.Application.Abilities;

public interface IAbilityQuerier
{
  Task<AbilityId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<AbilityModel> ReadAsync(Ability ability, CancellationToken cancellationToken = default);
  Task<AbilityModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<AbilityModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
