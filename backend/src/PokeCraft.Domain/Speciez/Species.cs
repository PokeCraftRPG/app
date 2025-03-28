using Logitar.EventSourcing;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Speciez.Events;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Domain.Speciez;

public class Species : AggregateRoot, IResource
{
  private SpeciesUpdated _updated = new();
  private bool HasUpdates => _updated.UniqueName is not null || _updated.DisplayName is not null
    || _updated.BaseFriendship is not null || _updated.CatchRate is not null || _updated.GrowthRate is not null
    || _updated.Link is not null || _updated.Notes is not null;

  public new SpeciesId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public ResourceType ResourceType => ResourceType.Species;
  public Guid EntityId => Id.EntityId;

  private SpeciesNumber? _number = null;
  public SpeciesNumber Number => _number ?? throw new InvalidOperationException("The species has not been initialized.");
  public SpeciesCategory Category { get; private set; }

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName
  {
    get => _uniqueName ?? throw new InvalidOperationException("The species has not been initialized.");
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

  private Friendship? _baseFriendship = null;
  public Friendship BaseFriendship
  {
    get => _baseFriendship ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_baseFriendship != value)
      {
        _baseFriendship = value;
        _updated.BaseFriendship = value;
      }
    }
  }
  private CatchRate? _catchRate = null;
  public CatchRate? CatchRate
  {
    get => _catchRate ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_catchRate != value)
      {
        _catchRate = value;
        _updated.CatchRate = value;
      }
    }
  }
  private GrowthRate? _growthRate = null;
  public GrowthRate GrowthRate
  {
    get => _growthRate ?? throw new InvalidOperationException("The species has not been initialized.");
    set
    {
      if (_growthRate != value)
      {
        _growthRate = value;
        _updated.GrowthRate = value;
      }
    }
  }

  private readonly Dictionary<RegionId, SpeciesNumber> _regionalNumbers = [];
  public IReadOnlyDictionary<RegionId, SpeciesNumber> RegionalNumbers => _regionalNumbers.AsReadOnly();

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

  public long Size => UniqueName.Size + (DisplayName?.Size ?? 0) + (Link?.Size ?? 0) + (Notes?.Size ?? 0);

  public Species() : base()
  {
  }

  public Species(SpeciesNumber number, SpeciesCategory category, UniqueName uniqueName, Friendship baseFriendship, CatchRate catchRate, GrowthRate growthRate, UserId userId, SpeciesId id)
    : base(id.StreamId)
  {
    if (!Enum.IsDefined(category))
    {
      throw new ArgumentOutOfRangeException(nameof(category));
    }
    if (!Enum.IsDefined(growthRate))
    {
      throw new ArgumentOutOfRangeException(nameof(growthRate));
    }

    Raise(new SpeciesCreated(number, category, uniqueName, baseFriendship, catchRate, growthRate), userId.ActorId);
  }
  protected virtual void Handle(SpeciesCreated @event)
  {
    _number = @event.Number;
    Category = @event.Category;

    _uniqueName = @event.UniqueName;

    _baseFriendship = @event.BaseFriendship;
    _catchRate = @event.CatchRate;
    _growthRate = @event.GrowthRate;
  }

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new SpeciesDeleted(), userId.ActorId);
    }
  }

  public void RemoveRegionalNumber(Region region, UserId userId) => RemoveRegionalNumber(region.Id, userId);
  public void RemoveRegionalNumber(RegionId regionId, UserId userId)
  {
    if (_regionalNumbers.ContainsKey(regionId))
    {
      Raise(new SpeciesRegionalNumberChanged(regionId, Number: null), userId.ActorId);
    }
  }
  public void SetRegionalNumber(Region region, SpeciesNumber number, UserId userId) => SetRegionalNumber(region.Id, number, userId);
  public void SetRegionalNumber(RegionId regionId, SpeciesNumber number, UserId userId)
  {
    if (WorldId != regionId.WorldId)
    {
      throw new ArgumentException($"The world '{regionId.WorldId}' was not expected ({WorldId}).", nameof(regionId));
    }

    if (!_regionalNumbers.TryGetValue(regionId, out SpeciesNumber? existingNumber) || existingNumber != number)
    {
      Raise(new SpeciesRegionalNumberChanged(regionId, number), userId.ActorId);
    }
  }
  protected virtual void Handle(SpeciesRegionalNumberChanged @event)
  {
    if (@event.Number is null)
    {
      _regionalNumbers.Remove(@event.RegionId);
    }
    else
    {
      _regionalNumbers[@event.RegionId] = @event.Number;
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
  protected virtual void Handle(SpeciesUpdated @event)
  {
    if (@event.UniqueName is not null)
    {
      _uniqueName = @event.UniqueName;
    }
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }

    if (@event.BaseFriendship is not null)
    {
      _baseFriendship = @event.BaseFriendship;
    }
    if (@event.CatchRate is not null)
    {
      _catchRate = @event.CatchRate;
    }
    if (@event.GrowthRate.HasValue)
    {
      _growthRate = @event.GrowthRate.Value;
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
