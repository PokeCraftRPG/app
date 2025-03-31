using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Application;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Queriers;

internal class RegionQuerier : IRegionQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<RegionEntity> _regions;

  public RegionQuerier(IActorService actorService, IApplicationContext applicationContext, PokemonContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _regions = context.Regions;
  }

  public async Task<RegionId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _regions.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new RegionId(streamId);
  }

  public async Task<IReadOnlyDictionary<Guid, string>> GetUniqueNameByIdsAsync(CancellationToken cancellationToken)
  {
    Dictionary<Guid, string> uniqueNameByIds = await _regions.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .Select(x => new { x.Id, x.UniqueName })
      .ToDictionaryAsync(x => x.Id, x => x.UniqueName, cancellationToken);

    return uniqueNameByIds.AsReadOnly();
  }

  public async Task<RegionModel> ReadAsync(Region region, CancellationToken cancellationToken)
  {
    return await ReadAsync(region.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The region entity 'StreamId={region.Id}' could not be found.");
  }
  public async Task<RegionModel?> ReadAsync(RegionId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    RegionEntity? region = await _regions.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return region is null ? null : await MapAsync(region, cancellationToken);
  }
  public async Task<RegionModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RegionEntity? region = await _regions.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return region is null ? null : await MapAsync(region, cancellationToken);
  }
  public async Task<RegionModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    RegionEntity? region = await _regions.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return region is null ? null : await MapAsync(region, cancellationToken);
  }

  private async Task<RegionModel> MapAsync(RegionEntity region, CancellationToken cancellationToken)
  {
    return (await MapAsync([region], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RegionModel>> MapAsync(IEnumerable<RegionEntity> regions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = regions.SelectMany(region => region.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    WorldModel world = _applicationContext.World;
    return regions.Select(region => region.World is null ? mapper.ToRegion(region, world) : mapper.ToRegion(region)).ToList().AsReadOnly();
  }
}
