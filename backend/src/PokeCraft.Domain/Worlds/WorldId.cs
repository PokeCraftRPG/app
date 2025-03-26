using Logitar;
using Logitar.EventSourcing;

namespace PokeCraft.Domain.Worlds;

public readonly struct WorldId
{
  private const string EntityType = "World";
  private const char Separator = ':';

  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId(string value)
  {
    StreamId = new StreamId(value);
  }
  public WorldId(Guid id)
  {
    string value = string.Join(Separator, EntityType, Convert.ToBase64String(id.ToByteArray()).ToUriSafeBase64());
    StreamId = new StreamId(value);
  }
  public WorldId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public static WorldId NewId() => new(Guid.NewGuid());

  public Guid ToGuid()
  {
    string[] parts = Value.Split(Separator);
    return new Guid(Convert.FromBase64String(parts.Last().FromUriSafeBase64()));
  }

  public static bool operator ==(WorldId left, WorldId right) => left.Equals(right);
  public static bool operator !=(WorldId left, WorldId right) => left != right;

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is WorldId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
