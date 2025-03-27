using PokeCraft.Application.Settings;
using PokeCraft.Application.Worlds;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Permissions;

public interface IPermissionService
{
  Task EnsureCanCreateAsync(ResourceType resourceType, CancellationToken cancellationToken = default);
  Task EnsureCanUpdateAsync(IResource resource, CancellationToken cancellationToken = default);
  Task EnsureCanUpdateAsync(World world, CancellationToken cancellationToken = default);
  Task EnsureCanViewAsync(ResourceType resourceType, CancellationToken cancellationToken = default);
  Task EnsureCanViewAsync(WorldModel world, CancellationToken cancellationToken = default);
}

internal class PermissionService : IPermissionService
{
  private readonly AccountSettings _accountSettings;
  private readonly IApplicationContext _applicationContext;
  private readonly IWorldQuerier _worldQuerier;

  public PermissionService(AccountSettings accountSettings, IApplicationContext applicationContext, IWorldQuerier worldQuerier)
  {
    _accountSettings = accountSettings;
    _applicationContext = applicationContext;
    _worldQuerier = worldQuerier;
  }

  public async Task EnsureCanCreateAsync(ResourceType resourceType, CancellationToken cancellationToken)
  {
    UserId userId = _applicationContext.UserId;
    if (resourceType == ResourceType.World)
    {
      int count = await _worldQuerier.CountAsync(userId, cancellationToken);
      if (count >= _accountSettings.WorldLimit)
      {
        throw new PermissionDeniedException(userId, ActionKind.Create, resourceType);
      }
    }
    else
    {
      if (!IsOwner(userId, _applicationContext.World))
      {
        throw new PermissionDeniedException(userId, ActionKind.Create, resourceType, _applicationContext.WorldId);
      }
    }
  }

  public Task EnsureCanUpdateAsync(IResource resource, CancellationToken cancellationToken)
  {
    UserId userId = _applicationContext.UserId;
    WorldModel world = _applicationContext.World;
    if (resource.WorldId.ToGuid() != world.Id || !IsOwner(userId, world))
    {
      throw new PermissionDeniedException(userId, ActionKind.Update, resource, _applicationContext.WorldId);
    }

    return Task.CompletedTask;
  }
  public Task EnsureCanUpdateAsync(World world, CancellationToken cancellationToken)
  {
    UserId userId = _applicationContext.UserId;
    if (!IsOwner(userId, world))
    {
      throw new PermissionDeniedException(userId, ActionKind.Update, Resource.From(world));
    }

    return Task.CompletedTask;
  }

  public Task EnsureCanViewAsync(ResourceType resourceType, CancellationToken cancellationToken)
  {
    UserId userId = _applicationContext.UserId;
    if (!IsOwner(userId, _applicationContext.World))
    {
      throw new PermissionDeniedException(userId, ActionKind.Read, resourceType, _applicationContext.WorldId);
    }

    return Task.CompletedTask;
  }
  public Task EnsureCanViewAsync(WorldModel world, CancellationToken cancellationToken)
  {
    UserId userId = _applicationContext.UserId;
    if (!IsOwner(userId, world))
    {
      throw new PermissionDeniedException(userId, ActionKind.Read, Resource.From(world));
    }

    return Task.CompletedTask;
  }

  private static bool IsOwner(UserId userId, World world) => world.OwnerId == userId;
  private static bool IsOwner(UserId userId, WorldModel world) => world.Owner.Id == userId.ToGuid();
}
