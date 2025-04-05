using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain.Moves;

public readonly struct MoveId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public MoveId(string value) : this(new StreamId(value))
  {
  }
  public MoveId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<WorldId, Guid> components = IdHelper.Deconstruct(streamId, ResourceType.Move);
    WorldId = components.Item1;
    EntityId = components.Item2;
  }
  public MoveId(WorldId worldId, Guid entityId)
  {
    StreamId = IdHelper.Construct(worldId, ResourceType.Move, entityId);

    WorldId = worldId;
    EntityId = entityId;
  }

  public static MoveId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(MoveId left, MoveId right) => left.Equals(right);
  public static bool operator !=(MoveId left, MoveId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is MoveId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
