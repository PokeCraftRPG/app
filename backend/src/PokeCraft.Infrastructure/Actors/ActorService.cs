using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts;
using PokeCraft.Infrastructure.Caching;

namespace PokeCraft.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}

internal class ActorService : IActorService
{
  private readonly ICacheService _cacheService;
  private readonly IUserService _userService;

  public ActorService(ICacheService cacheService, IUserService userService)
  {
    _cacheService = cacheService;
    _userService = userService;
  }

  public async Task<IReadOnlyCollection<ActorModel>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, ActorModel> actors = new(capacity);

    HashSet<Guid> missingIds = new(capacity);
    foreach (ActorId id in ids)
    {
      ActorModel? actor = _cacheService.GetActor(id);
      if (actor is null)
      {
        missingIds.Add(id.ToGuid());
      }
      else
      {
        actors[id] = actor;
      }
    }

    if (missingIds.Count > 0)
    {
      IReadOnlyCollection<UserModel> users = await _userService.FindAsync(missingIds, cancellationToken);
      foreach (UserModel user in users)
      {
        ActorModel actor = new(user);
        ActorId id = new(actor.Id);
        actors[id] = actor;
      }
    }

    foreach (ActorModel actor in actors.Values)
    {
      _cacheService.SetActor(actor);
    }

    return actors.Values;
  }
}
