using FluentValidation;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Application.Speciez.Validators;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez.Commands;

public record CreateOrReplaceSpeciesResult(SpeciesModel Species, bool Created);

public record CreateOrReplaceSpeciesCommand(Guid? Id, CreateOrReplaceSpeciesPayload Payload) : IRequest<CreateOrReplaceSpeciesResult>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceSpeciesCommandHandler : IRequestHandler<CreateOrReplaceSpeciesCommand, CreateOrReplaceSpeciesResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPermissionService _permissionService;
  private readonly ISpeciesManager _speciesManager;
  private readonly ISpeciesQuerier _speciesQuerier;
  private readonly ISpeciesRepository _speciesRepository;

  public CreateOrReplaceSpeciesCommandHandler(
    IApplicationContext applicationContext,
    IPermissionService permissionService,
    ISpeciesManager speciesManager,
    ISpeciesQuerier speciesQuerier,
    ISpeciesRepository speciesRepository)
  {
    _applicationContext = applicationContext;
    _permissionService = permissionService;
    _speciesManager = speciesManager;
    _speciesQuerier = speciesQuerier;
    _speciesRepository = speciesRepository;
  }

  public async Task<CreateOrReplaceSpeciesResult> Handle(CreateOrReplaceSpeciesCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpeciesPayload payload = command.Payload;
    new CreateOrReplaceSpeciesValidator().ValidateAndThrow(payload);

    SpeciesId id = SpeciesId.NewId(_applicationContext.WorldId);
    Species? species = null;
    if (command.Id.HasValue)
    {
      id = new(id.WorldId, command.Id.Value);
      species = await _speciesRepository.LoadAsync(id, cancellationToken);
    }

    UserId userId = _applicationContext.UserId;
    UniqueName uniqueName = new(payload.UniqueName);
    Friendship baseFriendship = new(payload.BaseFriendship);
    CatchRate catchRate = new(payload.CatchRate);

    bool created = false;
    if (species is null)
    {
      await _permissionService.EnsureCanCreateAsync(ResourceType.Species, cancellationToken);

      SpeciesNumber number = new(payload.Number);
      species = new(number, payload.Category, uniqueName, baseFriendship, catchRate, payload.GrowthRate, userId, id);
      created = true;
    }
    else
    {
      await _permissionService.EnsureCanUpdateAsync(species, cancellationToken);

      species.UniqueName = uniqueName;

      species.BaseFriendship = baseFriendship;
      species.CatchRate = catchRate;
      species.GrowthRate = payload.GrowthRate;
    }

    species.DisplayName = DisplayName.TryCreate(payload.DisplayName);

    // TODO(fpion): set regional numbers with permission check

    species.Link = Url.TryCreate(payload.Link);
    species.Notes = Notes.TryCreate(payload.Notes);

    species.Update(userId);
    await _speciesManager.SaveAsync(species, cancellationToken);

    SpeciesModel model = await _speciesQuerier.ReadAsync(species, cancellationToken);
    return new CreateOrReplaceSpeciesResult(model, created);
  }
}
