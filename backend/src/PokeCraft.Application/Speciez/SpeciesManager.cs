using Logitar.EventSourcing;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;
using PokeCraft.Domain.Speciez.Events;

namespace PokeCraft.Application.Speciez;

public interface ISpeciesManager
{
  Task SaveAsync(Species species, CancellationToken cancellationToken = default);
}

internal class SpeciesManager : ISpeciesManager
{
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;
  private readonly IStorageService _storageService;

  public SpeciesManager(ISpeciesQuerier speciesQuerier, ISpeciesRepository speciesRepository, IStorageService storageService)
  {
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
    _storageService = storageService;
  }

  public async Task SaveAsync(Species species, CancellationToken cancellationToken)
  {
    UniqueName? uniqueName = null;
    foreach (IEvent change in species.Changes)
    {
      if (change is SpeciesCreated created)
      {
        SpeciesId? conflictId = await _speciesQuerier.FindIdAsync(created.Number, cancellationToken);
        if (conflictId.HasValue && !conflictId.Value.Equals(species.Id))
        {
          // TODO(fpion): implement
        }

        uniqueName = created.UniqueName;
      }
      else if (change is SpeciesUpdated updated && updated.UniqueName is not null)
      {
        uniqueName = updated.UniqueName;
      }
      else if (change is SpeciesRegionalNumberChanged changed && changed.Number is not null)
      {
        // TODO(fpion): implement
      }
    }

    if (uniqueName is not null)
    {
      SpeciesId? conflictId = await _speciesQuerier.FindIdAsync(uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(species.Id))
      {
        throw new UniqueNameAlreadyUsedException(species, conflictId.Value);
      }
    }

    await _storageService.EnsureAvailableAsync(species, cancellationToken);

    await _speciesRepository.SaveAsync(species, cancellationToken);

    await _storageService.UpdateAsync(species, cancellationToken);
  }
}
