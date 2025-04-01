using Logitar.EventSourcing;
using PokeCraft.Application.Storages;
using PokeCraft.Domain.Storages;

namespace PokeCraft.Infrastructure.Repositories;

internal class StorageRepository : Repository, IStorageRepository
{
  public StorageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Storage?> LoadAsync(StorageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Storage>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Storage storage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storage, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Storage> storages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storages, cancellationToken);
  }
}
