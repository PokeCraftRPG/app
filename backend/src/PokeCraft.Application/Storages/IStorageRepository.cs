using PokeCraft.Domain.Storages;

namespace PokeCraft.Application.Storages;

public interface IStorageRepository
{
  Task<Storage?> LoadAsync(StorageId id, CancellationToken cancellationToken = default);

  Task SaveAsync(Storage storage, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Storage> storages, CancellationToken cancellationToken = default);
}
