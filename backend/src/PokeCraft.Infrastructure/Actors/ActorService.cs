using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using PokeCraft.Infrastructure.Caching;

namespace PokeCraft.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}

internal class ActorService : IActorService
{
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public async Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, ActorModel> actors = new(capacity);

    HashSet<ActorId> missingIds = new(capacity);
    foreach (ActorId id in ids)
    {
      ActorModel? actor = _cacheService.GetActor(id);
      if (actor is null)
      {
        missingIds.Add(id);
      }
      else
      {
        actors[id] = actor;
      }
    }

    if (missingIds.Count > 0)
    {
      await Task.Delay(1, cancellationToken); // TODO(fpion): implement
    }

    foreach (ActorModel actor in actors.Values)
    {
      _cacheService.SetActor(actor);
    }

    return actors.Values;
  }
}
