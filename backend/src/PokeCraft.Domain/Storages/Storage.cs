using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Domain.Storages.Events;

namespace PokeCraft.Domain.Storages;

public class Storage : AggregateRoot
{
  public new StorageId Id => new(base.Id);

  public UserId UserId { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes => _storedResources.Values.Sum();
  public long AvailableBytes => AllocatedBytes - UsedBytes;

  private readonly Dictionary<string, long> _storedResources = [];

  public Storage() : base()
  {
  }

  private Storage(UserId userId, long allocatedBytes) : base(new StorageId(userId).StreamId)
  {
    Raise(new StorageInitialized(userId, allocatedBytes), userId.ActorId);
  }
  protected virtual void Handle(StorageInitialized @event)
  {
    UserId = @event.UserId;

    AllocatedBytes = @event.AllocatedBytes;
  }

  public static Storage Initialize(UserId userId, long allocatedBytes) => new(userId, allocatedBytes);

  public long? GetSize(IResource resource)
  {
    string key = GetKey(resource);
    return _storedResources.TryGetValue(key, out long size) ? size : null;
  }

  public void Store(IResource resource)
  {
    string key = GetKey(resource);
    Raise(new ResourceStored(key, resource.Size), UserId.ActorId);
  }
  protected virtual void Handle(ResourceStored @event)
  {
    _storedResources[@event.Key] = @event.Size;
  }

  private static string GetKey(IResource resource) => $"{resource.WorldId}|{resource.ResourceType}:{Convert.ToBase64String(resource.EntityId.ToByteArray()).ToUriSafeBase64()}";
}
