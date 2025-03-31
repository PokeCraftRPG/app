using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;

namespace PokeCraft.Infrastructure.Caching;

public interface ICacheService
{
  ActorModel? GetActor(ActorId id);
}

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public ActorModel? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return _memoryCache.TryGetValue(key, out ActorModel? actor) ? actor : null;
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";
}
