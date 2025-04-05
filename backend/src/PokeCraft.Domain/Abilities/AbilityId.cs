using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain.Abilities;

public readonly struct AbilityId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public AbilityId(string value) : this(new StreamId(value))
  {
  }
  public AbilityId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<WorldId, Guid> components = IdHelper.Deconstruct(streamId, ResourceType.Ability);
    WorldId = components.Item1;
    EntityId = components.Item2;
  }
  public AbilityId(WorldId worldId, Guid entityId)
  {
    StreamId = IdHelper.Construct(worldId, ResourceType.Ability, entityId);

    WorldId = worldId;
    EntityId = entityId;
  }

  public static AbilityId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(AbilityId left, AbilityId right) => left.Equals(right);
  public static bool operator !=(AbilityId left, AbilityId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is AbilityId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
