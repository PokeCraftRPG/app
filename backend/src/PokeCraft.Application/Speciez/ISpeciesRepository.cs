using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez;

public interface ISpeciesRepository
{
  Task<Species?> LoadAsync(SpeciesId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Species species, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Species> species, CancellationToken cancellationToken = default);
}
