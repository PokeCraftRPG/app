using FluentValidation;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Regions.Validators;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;

namespace PokeCraft.Application.Regions.Commands;

public record UpdateRegionCommand(Guid Id, UpdateRegionPayload Payload) : IRequest<RegionModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand, RegionModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPermissionService _permissionService;
  private readonly IRegionManager _regionManager;
  private readonly IRegionQuerier _regionQuerier;
  private readonly IRegionRepository _regionRepository;

  public UpdateRegionCommandHandler(
    IApplicationContext applicationContext,
    IPermissionService permissionService,
    IRegionManager regionManager,
    IRegionQuerier regionQuerier,
    IRegionRepository regionRepository)
  {
    _applicationContext = applicationContext;
    _permissionService = permissionService;
    _regionManager = regionManager;
    _regionQuerier = regionQuerier;
    _regionRepository = regionRepository;
  }

  public async Task<RegionModel?> Handle(UpdateRegionCommand command, CancellationToken cancellationToken)
  {
    UpdateRegionPayload payload = command.Payload;
    new UpdateRegionValidator().ValidateAndThrow(payload);

    RegionId id = new(_applicationContext.WorldId, command.Id);
    Region? region = await _regionRepository.LoadAsync(id, cancellationToken);
    if (region is null)
    {
      return null;
    }
    await _permissionService.EnsureCanUpdateAsync(region, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      region.UniqueName = new UniqueName(payload.UniqueName);
    }
    if (payload.DisplayName is not null)
    {
      region.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      region.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Link is not null)
    {
      region.Link = Url.TryCreate(payload.Link.Value);
    }
    if (payload.Notes is not null)
    {
      region.Notes = Notes.TryCreate(payload.Notes.Value);
    }

    region.Update(_applicationContext.UserId);
    await _regionManager.SaveAsync(region, cancellationToken);

    return await _regionQuerier.ReadAsync(region, cancellationToken);
  }
}
