using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Application.Permissions;
using PokeCraft.Domain;

namespace PokeCraft.Application.Abilities.Queries;

public record ReadAbilityQuery(Guid? Id, string? UniqueName) : IRequest<AbilityModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadAbilityQueryHandler : IRequestHandler<ReadAbilityQuery, AbilityModel?>
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IPermissionService _permissionService;

  public ReadAbilityQueryHandler(IAbilityQuerier abilityQuerier, IPermissionService permissionService)
  {
    _abilityQuerier = abilityQuerier;
    _permissionService = permissionService;
  }

  public async Task<AbilityModel?> Handle(ReadAbilityQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanViewAsync(ResourceType.Ability, cancellationToken);

    Dictionary<Guid, AbilityModel> abilities = new(capacity: 2);

    if (query.Id.HasValue)
    {
      AbilityModel? ability = await _abilityQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (ability is not null)
      {
        abilities[ability.Id] = ability;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      AbilityModel? ability = await _abilityQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (ability is not null)
      {
        abilities[ability.Id] = ability;
      }
    }

    if (abilities.Count > 1)
    {
      throw TooManyResultsException<AbilityModel>.ExpectedSingle(abilities.Count);
    }

    return abilities.Values.SingleOrDefault();
  }
}
