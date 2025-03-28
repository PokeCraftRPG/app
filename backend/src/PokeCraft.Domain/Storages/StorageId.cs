using Logitar.EventSourcing;

namespace PokeCraft.Domain.Storages;

public readonly struct StorageId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public StorageId(string value)
  {
    StreamId = new StreamId(value);
  }
  public StorageId(UserId userId)
  {
    StreamId = new($"Storage:{userId}");
  }
  public StorageId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static bool operator ==(StorageId left, StorageId right) => left.Equals(right);
  public static bool operator !=(StorageId left, StorageId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is StorageId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
