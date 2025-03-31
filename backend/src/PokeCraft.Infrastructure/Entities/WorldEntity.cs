using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds;
using PokeCraft.Domain.Worlds.Events;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Entities;

internal class WorldEntity : AggregateEntity
{
  public int WorldId { get; private set; }
  public Guid Id { get; private set; }

  public Guid OwnerId { get; private set; }

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => Helper.Normalize(UniqueSlug);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public List<AbilityEntity> Abilities { get; private set; } = [];
  public List<MoveEntity> Moves { get; private set; } = [];

  public WorldEntity(WorldCreated @event) : base(@event)
  {
    Id = new WorldId(@event.StreamId).ToGuid();

    OwnerId = @event.OwnerId.ToGuid();

    UniqueSlug = @event.UniqueSlug.Value;
  }

  private WorldEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 3);
    actorIds.AddRange(base.GetActorIds());
    actorIds.Add(new ActorId(OwnerId));
    return actorIds.ToList().AsReadOnly();
  }

  public void Update(WorldUpdated @event)
  {
    base.Update(@event);

    if (@event.UniqueSlug is not null)
    {
      UniqueSlug = @event.UniqueSlug.Value;
    }
    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
