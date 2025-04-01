using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;
using PokeCraft.Infrastructure.Settings;

namespace PokeCraft.Infrastructure.Caching;

public interface ICacheService
{
  ActorModel? GetActor(ActorId id);
  void SetActor(ActorModel actor);
}

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;
  private readonly CachingSettings _settings;

  public CacheService(IMemoryCache memoryCache, CachingSettings settings)
  {
    _memoryCache = memoryCache;
    _settings = settings;
  }

  public ActorModel? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return _memoryCache.TryGetValue(key, out ActorModel? actor) ? actor : null;
  }
  public void SetActor(ActorModel actor)
  {
    string key = GetActorKey(new ActorId(actor.Id));
    _memoryCache.Set(key, actor, _settings.ActorLifetime);
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";
}
