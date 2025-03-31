using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez;

public interface ISpeciesQuerier
{
  Task<SpeciesId?> FindIdAsync(SpeciesNumber number, CancellationToken cancellationToken = default);
  Task<SpeciesId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<SpeciesModel> ReadAsync(Species species, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(int number, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(int number, RegionModel? region, CancellationToken cancellationToken = default);
  Task<SpeciesModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
