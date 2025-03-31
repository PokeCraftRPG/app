using FluentValidation;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Regions.Validators;
using PokeCraft.Application.Storages;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;

namespace PokeCraft.Application.Regions.Commands;

public record CreateOrReplaceRegionResult(RegionModel Region, bool Created);

public record CreateOrReplaceRegionCommand(Guid? Id, CreateOrReplaceRegionPayload Payload) : IRequest<CreateOrReplaceRegionResult>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceRegionCommandHandler : IRequestHandler<CreateOrReplaceRegionCommand, CreateOrReplaceRegionResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IPermissionService _permissionService;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public CreateOrReplaceRegionCommandHandler(
    IApplicationContext applicationContext,
    IMediator mediator,
    IPermissionService permissionService,
    IRegionQuerier regionQuerier,
    IRegionRepository regionRepository)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _permissionService = permissionService;
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task<CreateOrReplaceRegionResult> Handle(CreateOrReplaceRegionCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegionPayload payload = command.Payload;
    new CreateOrReplaceRegionValidator().ValidateAndThrow(payload);

    RegionId id = RegionId.NewId(_applicationContext.WorldId);
    Region? region = null;
    if (command.Id.HasValue)
    {
      id = new(id.WorldId, command.Id.Value);
      region = await _regionRepository.LoadAsync(id, cancellationToken);
    }

    UserId userId = _applicationContext.UserId;
    UniqueName uniqueName = new(payload.UniqueName);

    bool created = false;
    if (region is null)
    {
      await _permissionService.EnsureCanCreateAsync(ResourceType.Region, cancellationToken);

      region = new(uniqueName, userId, id);
      created = true;
    }
    else
    {
      await _permissionService.EnsureCanUpdateAsync(region, cancellationToken);

      region.UniqueName = uniqueName;
    }

    region.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    region.Description = Description.TryCreate(payload.Description);

    region.Link = Url.TryCreate(payload.Link);
    region.Notes = Notes.TryCreate(payload.Notes);

    region.Update(userId);
    await _mediator.Send(new SaveRegionCommand(region), cancellationToken);

    RegionModel model = await _regionQuerier.ReadAsync(region, cancellationToken);
    return new CreateOrReplaceRegionResult(model, created);
  }
}
