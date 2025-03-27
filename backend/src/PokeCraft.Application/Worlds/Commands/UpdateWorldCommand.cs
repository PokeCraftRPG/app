using FluentValidation;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Storages;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Application.Worlds.Validators;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds.Commands;

public record UpdateWorldCommand(Guid Id, UpdateWorldPayload Payload) : IRequest<WorldModel?>;

/// <exception cref="NotEnoughAvailableStorageException"></exception>
/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="UniqueSlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class UpdateWorldCommandHandler : IRequestHandler<UpdateWorldCommand, WorldModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPermissionService _permissionService;
  private readonly IWorldManager _worldManager;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public UpdateWorldCommandHandler(
    IApplicationContext applicationContext,
    IPermissionService permissionService,
    IWorldManager worldManager,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository)
  {
    _applicationContext = applicationContext;
    _permissionService = permissionService;
    _worldManager = worldManager;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<WorldModel?> Handle(UpdateWorldCommand command, CancellationToken cancellationToken)
  {
    UpdateWorldPayload payload = command.Payload;
    new UpdateWorldValidator().ValidateAndThrow(payload);

    WorldId id = new(command.Id);
    World? world = await _worldRepository.LoadAsync(id, cancellationToken);
    if (world is null)
    {
      return null;
    }
    await _permissionService.EnsureCanUpdateAsync(world, cancellationToken);

    if (!string.IsNullOrWhiteSpace(payload.UniqueSlug))
    {
      world.UniqueSlug = new Slug(payload.UniqueSlug);
    }
    if (payload.DisplayName is not null)
    {
      world.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      world.Description = Description.TryCreate(payload.Description.Value);
    }

    world.Update(_applicationContext.UserId);
    await _worldManager.SaveAsync(world, cancellationToken);

    return await _worldQuerier.ReadAsync(world, cancellationToken);
  }
}
