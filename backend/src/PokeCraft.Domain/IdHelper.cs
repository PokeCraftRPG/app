using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain;

internal static class IdHelper
{
  private const char ComponentSeparator = ':';
  private const char Separator = '|';

  public static StreamId Construct(WorldId worldId, ResourceType resourceType, Guid entityId)
  {
    string value = string.Join(Separator, worldId, string.Join(ComponentSeparator, resourceType, Convert.ToBase64String(entityId.ToByteArray()).ToUriSafeBase64()));
    return new StreamId(value);
  }
  public static Tuple<WorldId, Guid> Deconstruct(StreamId streamId, ResourceType expectedType)
  {
    string[] parts = streamId.Value.Split(Separator);
    if (parts.Length != 2)
    {
      throw new ArgumentException("The value is not a valid entity ID.", nameof(streamId));
    }

    WorldId worldId = new(parts.First());

    string[] components = parts.Last().Split(ComponentSeparator);
    if (components.Length != 2)
    {
      throw new ArgumentException("The value is not a valid entity ID.", nameof(streamId));
    }
    string entityType = components.First();
    if (entityType != expectedType.ToString())
    {
      throw new ArgumentException($"The entity type '{entityType}' was not expected ({expectedType}).", nameof(streamId));
    }
    Guid entityId = new(Convert.FromBase64String(components.Last().FromUriSafeBase64()));

    return Tuple.Create(worldId, entityId);
  }
}
