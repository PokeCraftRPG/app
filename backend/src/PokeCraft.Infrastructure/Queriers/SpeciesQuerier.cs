using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Application;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Speciez;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Speciez;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Queriers;

internal class SpeciesQuerier : ISpeciesQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<SpeciesEntity> _species;

  public SpeciesQuerier(IActorService actorService, IApplicationContext applicationContext, PokemonContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _species = context.Species;
  }

  public async Task<SpeciesId?> FindIdAsync(SpeciesNumber number, CancellationToken cancellationToken)
  {
    string? streamId = await _species.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .Where(x => x.Number == number.Value)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new SpeciesId(streamId);
  }
  public async Task<SpeciesId?> FindIdAsync(SpeciesNumber number, RegionId? regionId, CancellationToken cancellationToken)
  {
    if (!regionId.HasValue)
    {
      return await FindIdAsync(number, cancellationToken);
    }

    throw new NotImplementedException(); // TODO(fpion): implement
  }
  public async Task<SpeciesId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _species.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new SpeciesId(streamId);
  }

  public async Task<SpeciesModel> ReadAsync(Species species, CancellationToken cancellationToken)
  {
    return await ReadAsync(species.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The species entity 'StreamId={species.Id}' could not be found.");
  }
  public async Task<SpeciesModel?> ReadAsync(SpeciesId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    SpeciesEntity? species = await _species.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<SpeciesModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _species.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<SpeciesModel?> ReadAsync(int number, CancellationToken cancellationToken)
  {
    SpeciesEntity? species = await _species.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.Number == number, cancellationToken);

    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<SpeciesModel?> ReadAsync(int number, RegionModel? region, CancellationToken cancellationToken)
  {
    if (region is null)
    {
      return await ReadAsync(number, cancellationToken);
    }

    throw new NotImplementedException(); // TODO(fpion): implement
  }
  public async Task<SpeciesModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    SpeciesEntity? species = await _species.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return species is null ? null : await MapAsync(species, cancellationToken);
  }

  private async Task<SpeciesModel> MapAsync(SpeciesEntity species, CancellationToken cancellationToken)
  {
    return (await MapAsync([species], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SpeciesModel>> MapAsync(IEnumerable<SpeciesEntity> speciez, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = speciez.SelectMany(species => species.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    WorldModel world = _applicationContext.World;
    return speciez.Select(species => species.World is null ? mapper.ToSpecies(species, world) : mapper.ToSpecies(species)).ToList().AsReadOnly();
  }
}
