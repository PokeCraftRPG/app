using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Speciez.Queries;

public record ReadSpeciesQuery(Guid? Id, int? Number, string? UniqueName, string? Region) : IRequest<SpeciesModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="RegionNotFoundException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadSpeciesQueryHandler : IRequestHandler<ReadSpeciesQuery, SpeciesModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPermissionService _permissionService;
  private readonly IRegionQuerier _regionQuerier;
  private readonly ISpeciesQuerier _speciesQuerier;

  public ReadSpeciesQueryHandler(
    IApplicationContext applicationContext,
    IPermissionService permissionService,
    IRegionQuerier regionQuerier,
    ISpeciesQuerier speciesQuerier)
  {
    _applicationContext = applicationContext;
    _permissionService = permissionService;
    _regionQuerier = regionQuerier;
    _speciesQuerier = speciesQuerier;
  }

  public async Task<SpeciesModel?> Handle(ReadSpeciesQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanViewAsync(ResourceType.Species, cancellationToken);

    Dictionary<Guid, SpeciesModel> foundSpecies = new(capacity: 3);

    if (query.Id.HasValue)
    {
      SpeciesModel? species = await _speciesQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (species is not null)
      {
        foundSpecies[species.Id] = species;
      }
    }
    if (query.Number.HasValue)
    {
      RegionModel? region = null;
      if (!string.IsNullOrWhiteSpace(query.Region))
      {
        await _permissionService.EnsureCanViewAsync(ResourceType.Region, cancellationToken);

        region = await FindRegionAsync(query.Region, cancellationToken);
      }

      SpeciesModel? species = await _speciesQuerier.ReadAsync(query.Number.Value, region, cancellationToken);
      if (species is not null)
      {
        foundSpecies[species.Id] = species;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      SpeciesModel? species = await _speciesQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (species is not null)
      {
        foundSpecies[species.Id] = species;
      }
    }

    if (foundSpecies.Count > 1)
    {
      throw TooManyResultsException<SpeciesModel>.ExpectedSingle(foundSpecies.Count);
    }

    return foundSpecies.Values.SingleOrDefault();
  }

  private async Task<RegionModel> FindRegionAsync(string idOrUniqueName, CancellationToken cancellationToken)
  {
    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      RegionModel? region = await _regionQuerier.ReadAsync(id, cancellationToken);
      if (region is not null)
      {
        return region;
      }
    }

    return await _regionQuerier.ReadAsync(idOrUniqueName, cancellationToken)
      ?? throw new RegionNotFoundException(_applicationContext.WorldId, idOrUniqueName, "region");
  }
}
