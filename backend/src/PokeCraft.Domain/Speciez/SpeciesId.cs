using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain.Speciez;

public readonly struct SpeciesId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public WorldId WorldId { get; }
  public Guid EntityId { get; }

  public SpeciesId(string value) : this(new StreamId(value))
  {
  }
  public SpeciesId(StreamId streamId)
  {
    StreamId = streamId;

    Tuple<WorldId, Guid> components = IdHelper.Deconstruct(streamId, ResourceType.Species);
    WorldId = components.Item1;
    EntityId = components.Item2;
  }
  public SpeciesId(WorldId worldId, Guid entityId)
  {
    StreamId = IdHelper.Construct(worldId, ResourceType.Species, entityId);

    WorldId = worldId;
    EntityId = entityId;
  }

  public static SpeciesId NewId(WorldId worldId) => new(worldId, Guid.NewGuid());

  public static bool operator ==(SpeciesId left, SpeciesId right) => left.Equals(right);
  public static bool operator !=(SpeciesId left, SpeciesId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is SpeciesId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}
