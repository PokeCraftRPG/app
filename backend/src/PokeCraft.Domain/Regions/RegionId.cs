using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain.Regions;

public readonly struct RegionId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public RegionId(string value) : this(new StreamId(value))
  {
  }
  public RegionId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<WorldId, Guid> components = IdHelper.Deconstruct(streamId, ResourceType.Region);
    WorldId = components.Item1;
    EntityId = components.Item2;
  }
  public RegionId(WorldId worldId, Guid entityId)
  {
    StreamId = IdHelper.Construct(worldId, ResourceType.Region, entityId);

    WorldId = worldId;
    EntityId = entityId;
  }

  public static RegionId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(RegionId left, RegionId right) => left.Equals(right);
  public static bool operator !=(RegionId left, RegionId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RegionId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
