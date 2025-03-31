using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Application;
using PokeCraft.Application.Abilities;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Queriers;

internal class AbilityQuerier : IAbilityQuerier
{
  private readonly DbSet<AbilityEntity> _abilities;
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;

  public AbilityQuerier(IActorService actorService, IApplicationContext applicationContext, PokemonContext context)
  {
    _abilities = context.Abilities;
    _actorService = actorService;
    _applicationContext = applicationContext;
  }

  public async Task<AbilityId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _abilities.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new AbilityId(streamId);
  }

  public async Task<AbilityModel> ReadAsync(Ability ability, CancellationToken cancellationToken)
  {
    return await ReadAsync(ability.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The ability entity 'StreamId={ability.Id}' could not be found.");
  }
  public async Task<AbilityModel?> ReadAsync(AbilityId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    AbilityEntity? ability = await _abilities.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return ability is null ? null : await MapAsync(ability, cancellationToken);
  }
  public async Task<AbilityModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    AbilityEntity? ability = await _abilities.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return ability is null ? null : await MapAsync(ability, cancellationToken);
  }
  public async Task<AbilityModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    AbilityEntity? ability = await _abilities.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return ability is null ? null : await MapAsync(ability, cancellationToken);
  }

  private async Task<AbilityModel> MapAsync(AbilityEntity ability, CancellationToken cancellationToken)
  {
    return (await MapAsync([ability], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<AbilityModel>> MapAsync(IEnumerable<AbilityEntity> abilities, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = abilities.SelectMany(ability => ability.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    WorldModel world = _applicationContext.World;
    return abilities.Select(ability => ability.World is null ? mapper.ToAbility(ability, world) : mapper.ToAbility(ability)).ToList().AsReadOnly();
  }
}
