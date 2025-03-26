using Logitar.EventSourcing;
using PokeCraft.Domain.Worlds.Events;

namespace PokeCraft.Domain.Worlds;

public class World : AggregateRoot
{
  private WorldUpdated _updated = new();
  private bool HasUpdates => _updated.UniqueSlug is not null || _updated.DisplayName is not null || _updated.Description is not null;

  public new WorldId Id => new(base.Id);

  public UserId OwnerId { get; private set; }

  private Slug? _uniqueSlug = null;
  public Slug UniqueSlug
  {
    get => _uniqueSlug ?? throw new InvalidOperationException("The world has not been initialized.");
    set
    {
      if (_uniqueSlug != value)
      {
        _uniqueSlug = value;
        _updated.UniqueSlug = value;
      }
    }
  }
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  public World() : base()
  {
  }

  public World(UserId ownerId, Slug uniqueSlug, WorldId id) : base(id.StreamId)
  {
    Raise(new WorldCreated(ownerId, uniqueSlug), ownerId.ActorId);
  }
  protected virtual void Handle(WorldCreated @event)
  {
    OwnerId = @event.OwnerId;

    _uniqueSlug = @event.UniqueSlug;
  }

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new WorldDeleted(), userId.ActorId);
    }
  }

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(WorldUpdated @event)
  {
    if (@event.UniqueSlug is not null)
    {
      _uniqueSlug = @event.UniqueSlug;
    }
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueSlug.Value} | {base.ToString()}";
}
