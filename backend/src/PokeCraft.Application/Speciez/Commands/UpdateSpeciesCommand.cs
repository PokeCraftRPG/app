﻿using FluentValidation;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions;
using PokeCraft.Application.Regions.Queries;
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
/// <exception cref="RegionsNotFoundException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateSpeciesCommandHandler : IRequestHandler<UpdateSpeciesCommand, SpeciesModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPermissionService _permissionService;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public UpdateSpeciesCommandHandler(
    IApplicationContext applicationContext,
    IMediator mediator,
    IPermissionService permissionService,
    ISpeciesQuerier speciesQuerier,
    ISpeciesRepository speciesRepository)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _permissionService = permissionService;
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
    await _mediator.Send(new SaveSpeciesCommand(species), cancellationToken);

    return await _speciesQuerier.ReadAsync(species, cancellationToken);
  }

  private async Task UpdateRegionalNumbersAsync(Species species, UpdateSpeciesPayload payload, UserId userId, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanViewAsync(ResourceType.Region, cancellationToken);

    IEnumerable<string> idOrUniqueNames = payload.RegionalNumbers.Select(x => x.Region);
    string propertyName = string.Join('.', nameof(payload.RegionalNumbers), nameof(RegionalNumberUpdatePayload.Region));
    IReadOnlyDictionary<string, Region> regions = await _mediator.Send(new FindRegionsQuery(idOrUniqueNames, propertyName), cancellationToken);

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
