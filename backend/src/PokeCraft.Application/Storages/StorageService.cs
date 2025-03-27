using PokeCraft.Application.Settings;
using PokeCraft.Application.Worlds;
using PokeCraft.Domain;
using PokeCraft.Domain.Storages;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Storages;

public interface IStorageService
{
  Task EnsureAvailableAsync(IResource resource, CancellationToken cancellationToken = default);
  Task UpdateAsync(IResource resource, CancellationToken cancellationToken = default);
}

internal class StorageService : IStorageService
{
  private readonly Dictionary<WorldId, Storage> _cache = [];

  private readonly AccountSettings _accountSettings;
  private readonly IStorageRepository _storageRepository;
  private readonly IWorldQuerier _worldQuerier;

  public StorageService(AccountSettings accountSettings, IStorageRepository storageRepository, IWorldQuerier worldQuerier)
  {
    _accountSettings = accountSettings;
    _storageRepository = storageRepository;
    _worldQuerier = worldQuerier;
  }

  public async Task EnsureAvailableAsync(IResource resource, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(resource, cancellationToken);

    long previousBytes = storage.GetSize(resource) ?? 0;
    long requiredBytes = resource.Size - previousBytes;
    if (requiredBytes > 0 && requiredBytes > storage.AvailableBytes)
    {
      throw new NotEnoughAvailableStorageException(storage, requiredBytes);
    }
  }

  public async Task UpdateAsync(IResource resource, CancellationToken cancellationToken)
  {
    Storage storage = await LoadOrInitializeAsync(resource, cancellationToken);

    storage.Store(resource);

    await _storageRepository.SaveAsync(storage, cancellationToken);
  }

  private async Task<Storage> LoadOrInitializeAsync(IResource resource, CancellationToken cancellationToken)
  {
    WorldId worldId = resource.WorldId;
    if (_cache.TryGetValue(worldId, out Storage? storage))
    {
      return storage;
    }

    UserId ownerId = await _worldQuerier.FindOwnerIdAsync(worldId, cancellationToken);
    StorageId id = new(ownerId);
    storage = await _storageRepository.LoadAsync(id, cancellationToken) ?? Storage.Initialize(ownerId, _accountSettings.AllocatedBytes);

    _cache[worldId] = storage;

    return storage;
  }
}
