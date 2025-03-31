using FluentValidation;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Application.Speciez.Validators;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez.Commands;

public record UpdateSpeciesCommand(Guid Id, UpdateSpeciesPayload Payload) : IRequest<SpeciesModel?>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateSpeciesCommandHandler : IRequestHandler<UpdateSpeciesCommand, SpeciesModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPermissionService _permissionService;
  private readonly IRegionManager _regionManager;
  private readonly ISpeciesManager _speciesManager;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public UpdateSpeciesCommandHandler(
    IApplicationContext applicationContext,
    IPermissionService permissionService,
    IRegionManager regionManager,
    ISpeciesManager speciesManager,
    ISpeciesQuerier speciesQuerier,
    ISpeciesRepository speciesRepository)
  {
    _applicationContext = applicationContext;
    _permissionService = permissionService;
    _regionManager = regionManager;
    _speciesManager = speciesManager;
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
  }

  public async Task<SpeciesModel?> Handle(UpdateSpeciesCommand command, CancellationToken cancellationToken)
  {
    UpdateSpeciesPayload payload = command.Payload;
    new UpdateSpeciesValidator().ValidateAndThrow(payload);

    SpeciesId id = new(_applicationContext.WorldId, command.Id);
    Species? species = await _speciesRepository.LoadAsync(id, cancellationToken);
    if (species is null)
    {
      return null;
    }
    await _permissionService.EnsureCanUpdateAsync(species, cancellationToken);

    UserId userId = _applicationContext.UserId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      species.UniqueName = new UniqueName(payload.UniqueName);
    }
    if (payload.DisplayName is not null)
    {
      species.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }

    if (payload.BaseFriendship.HasValue)
    {
      species.BaseFriendship = new Friendship(payload.BaseFriendship.Value);
    }
    if (payload.CatchRate.HasValue)
    {
      species.CatchRate = new CatchRate(payload.CatchRate.Value);
    }
    if (payload.GrowthRate.HasValue)
    {
      species.GrowthRate = payload.GrowthRate.Value;
    }

    if (payload.RegionalNumbers.Count > 0)
    {
      await UpdateRegionalNumbersAsync(species, payload, userId, cancellationToken);
    }

    if (payload.Link is not null)
    {
      species.Link = Url.TryCreate(payload.Link.Value);
    }
    if (payload.Notes is not null)
    {
      species.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    species.Update(userId);
    await _speciesManager.SaveAsync(species, cancellationToken);

    return await _speciesQuerier.ReadAsync(species, cancellationToken);
  }

  private async Task UpdateRegionalNumbersAsync(Species species, UpdateSpeciesPayload payload, UserId userId, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanViewAsync(ResourceType.Region, cancellationToken);

    IEnumerable<string> idOrUniqueNames = payload.RegionalNumbers.Select(x => x.Region);
    IReadOnlyDictionary<string, Region> regions = await _regionManager.FindAsync(idOrUniqueNames, nameof(payload.RegionalNumbers), cancellationToken);

    foreach (RegionalNumberUpdatePayload regional in payload.RegionalNumbers)
    {
      Region region = regions[regional.Region];
      if (regional.Number.HasValue)
      {
        SpeciesNumber number = new(regional.Number.Value);
        species.SetRegionalNumber(region, number, userId);
      }
      else
      {
        species.RemoveRegionalNumber(region, userId);
      }
    }
  }
}
