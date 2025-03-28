using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Permissions;

internal record Resource(WorldId WorldId, ResourceType ResourceType, Guid EntityId, long Size) : IResource
{
  public static Resource From(World world) => new(world.Id, ResourceType.World, world.Id.ToGuid(), world.Size);
  public static Resource From(WorldModel world) => new(new WorldId(world.Id), ResourceType.World, world.Id, Size: 0);
}
