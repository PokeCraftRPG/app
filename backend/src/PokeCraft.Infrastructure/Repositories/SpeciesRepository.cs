using Logitar.EventSourcing;
using PokeCraft.Application.Speciez;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Infrastructure.Repositories;

internal class SpeciesRepository : Repository, ISpeciesRepository
{
  public SpeciesRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Species?> LoadAsync(SpeciesId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Species>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Species species, CancellationToken cancellationToken)
  {
    await base.SaveAsync(species, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Species> abilities, CancellationToken cancellationToken)
  {
    await base.SaveAsync(abilities, cancellationToken);
  }
}
