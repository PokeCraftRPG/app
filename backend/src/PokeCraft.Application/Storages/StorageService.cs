using PokeCraft.Application.Permissions;
using PokeCraft.Application.Settings;
using PokeCraft.Domain;
using PokeCraft.Domain.Storages;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Storages;

public interface IStorageService
{
  Task EnsureAvailableAsync(IResource resource, CancellationToken cancellationToken = default);
  Task EnsureAvailableAsync(World world, CancellationToken cancellationToken = default);

  Task UpdateAsync(IResource resource, CancellationToken cancellationToken = default);
  Task UpdateAsync(World world, CancellationToken cancellationToken = default);
}

internal class StorageService : IStorageService
{
  private readonly Dictionary<WorldId, Storage> _cache = [];

  private readonly AccountSettings _accountSettings;
  private readonly IApplicationContext _applicationContext;
  private readonly IStorageRepository _storageRepository;

  public StorageService(AccountSettings accountSettings, IApplicationContext applicationContext, IStorageRepository storageRepository)
  {
    _accountSettings = accountSettings;
    _applicationContext = applicationContext;
    _storageRepository = storageRepository;
  }

  public async Task EnsureAvailableAsync(IResource resource, CancellationToken cancellationToken)
  {
    UserId ownerId = new(_applicationContext.World.Owner.Id);
    Storage storage = await LoadOrInitializeAsync(resource.WorldId, ownerId, cancellationToken);

    EnsureAvailable(storage, resource);
  }
  public async Task EnsureAvailableAsync(World world, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(world.Id, world.OwnerId, cancellationToken);

    Resource resource = Resource.From(world);
    EnsureAvailable(storage, resource);
  }
  private static void EnsureAvailable(Storage storage, IResource resource)
  {
    long previousBytes = storage.GetSize(resource) ?? 0;
    long requiredBytes = resource.Size - previousBytes;
    if (requiredBytes > 0 && requiredBytes > storage.AvailableBytes)
    {
      throw new NotEnoughAvailableStorageException(storage, requiredBytes);
    }
  }

  public async Task UpdateAsync(IResource resource, CancellationToken cancellationToken)
  {
    UserId ownerId = new(_applicationContext.World.Owner.Id);
    Storage storage = await LoadOrInitializeAsync(resource.WorldId, ownerId, cancellationToken);

    await UpdateAsync(storage, resource, cancellationToken);
  }
  public async Task UpdateAsync(World world, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(world.Id, world.OwnerId, cancellationToken);

    Resource resource = Resource.From(world);
    await UpdateAsync(storage, resource, cancellationToken);
  }
  private async Task UpdateAsync(Storage storage, IResource resource, CancellationToken cancellationToken)
  {
    storage.Store(resource);

    await _storageRepository.SaveAsync(storage, cancellationToken);
  }

  private async Task<Storage> LoadOrInitializeAsync(WorldId worldId, UserId ownerId, CancellationToken cancellationToken)
  {
    if (_cache.TryGetValue(worldId, out Storage? storage))
    {
      return storage;
    }

    StorageId id = new(ownerId);
    storage = await _storageRepository.LoadAsync(id, cancellationToken) ?? Storage.Initialize(ownerId, _accountSettings.AllocatedBytes);

    _cache[worldId] = storage;

    return storage;
  }
}
