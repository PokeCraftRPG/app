using Logitar.EventSourcing;
using PokeCraft.Domain.Abilities.Events;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain.Abilities;

public class Ability : AggregateRoot, IResource
{
  private AbilityUpdated _updated = new();
  private bool HasUpdates => _updated.UniqueName is not null || _updated.DisplayName is not null || _updated.Description is not null
    || _updated.Link is not null || _updated.Notes is not null;

  public new AbilityId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public ResourceType ResourceType => ResourceType.Ability;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName
  {
    get => _uniqueName ?? throw new InvalidOperationException("The ability has not been initialized.");
    set
    {
      if (_uniqueName != value)
      {
        _uniqueName = value;
        _updated.UniqueName = value;
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

  private Url? _link = null;
  public Url? Link
  {
    get => _link;
    set
    {
      if (_link != value)
      {
        _link = value;
        _updated.Link = new Change<Url>(value);
      }
    }
  }
  private Notes? _notes = null;
  public Notes? Notes
  {
    get => _notes;
    set
    {
      if (_notes != value)
      {
        _notes = value;
        _updated.Notes = new Change<Notes>(value);
      }
    }
  }

  public long Size => UniqueName.Size + (DisplayName?.Size ?? 0) + (Description?.Size ?? 0) + (Link?.Size ?? 0) + (Notes?.Size ?? 0);

  public Ability() : base()
  {
  }

  public Ability(UniqueName uniqueName, UserId userId, AbilityId id) : base(id.StreamId)
  {
    Raise(new AbilityCreated(uniqueName), userId.ActorId);
  }
  protected virtual void Handle(AbilityCreated @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new AbilityDeleted(), userId.ActorId);
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
  protected virtual void Handle(AbilityUpdated @event)
  {
    if (@event.UniqueName is not null)
    {
      _uniqueName = @event.UniqueName;
    }
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }

    if (@event.Link is not null)
    {
      _link = @event.Link.Value;
    }
    if (@event.Notes is not null)
    {
      _notes = @event.Notes.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
