using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain;

public interface IResource
{
  /// <summary>
  /// Gets the identifier of the world in which the resource resides.
  /// </summary>
  WorldId WorldId { get; }
  /// <summary>
  /// Gets the type of the resource.
  /// </summary>
  ResourceType ResourceType { get; }
  /// <summary>
  /// Gets the identifier of the resource within its world.
  /// </summary>
  Guid EntityId { get; }
  /// <summary>
  /// Gets the size of the resource, in bytes.
  /// </summary>
  long Size { get; }
}
