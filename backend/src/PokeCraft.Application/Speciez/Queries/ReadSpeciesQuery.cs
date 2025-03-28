using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Speciez.Queries;

public record ReadSpeciesQuery(Guid? Id, int? Number, string? UniqueName, string? Region) : IRequest<SpeciesModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadSpeciesQueryHandler : IRequestHandler<ReadSpeciesQuery, SpeciesModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly ISpeciesQuerier _speciesQuerier;

  public ReadSpeciesQueryHandler(IPermissionService permissionService, ISpeciesQuerier speciesQuerier)
  {
    _permissionService = permissionService;
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
      // TODO(fpion): regional number search with permission check

      SpeciesModel? species = await _speciesQuerier.ReadAsync(query.Number.Value, cancellationToken);
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
}
