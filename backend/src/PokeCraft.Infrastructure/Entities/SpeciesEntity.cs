using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;
using PokeCraft.Domain.Speciez.Events;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Entities;

internal class SpeciesEntity : AggregateEntity, ISegregatedEntity
{
  public int SpeciesId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public int Number { get; private set; }
  public SpeciesCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }

  public int BaseFriendship { get; private set; }
  public int CatchRate { get; private set; }
  public GrowthRate GrowthRate { get; private set; }

  public string? Link { get; private set; }
  public string? Notes { get; private set; }

  public List<RegionalNumberEntity> RegionalNumbers { get; private set; } = [];

  public SpeciesEntity(WorldEntity world, SpeciesCreated @event) : base(@event)
  {
    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Id = new SpeciesId(@event.StreamId).EntityId;

    Number = @event.Number.Value;
    Category = @event.Category;

    UniqueName = @event.UniqueName.Value;

    BaseFriendship = @event.BaseFriendship.Value;
    CatchRate = @event.CatchRate.Value;
    GrowthRate = @event.GrowthRate;
  }

  private SpeciesEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 5);
    actorIds.AddRange(base.GetActorIds());
    if (World is not null)
    {
      actorIds.AddRange(World.GetActorIds());
    }
    return actorIds.ToList().AsReadOnly();
  }

  public void SetRegionalNumber(RegionEntity? region, SpeciesRegionalNumberChanged @event)
  {
    base.Update(@event);

    RegionalNumberEntity? entity = RegionalNumbers.SingleOrDefault(x => x.RegionUid == @event.RegionId.EntityId);
    if (entity is null)
    {
      if (@event.Number is not null)
      {
        ArgumentNullException.ThrowIfNull(region, nameof(region));
        entity = new RegionalNumberEntity(this, region, @event);
      }
    }
    else if (@event.Number is null)
    {
      RegionalNumbers.Remove(entity);
    }
    else
    {
      entity.Update(@event);
    }
  }

  public void Update(SpeciesUpdated @event)
  {
    base.Update(@event);

    if (@event.UniqueName is not null)
    {
      UniqueName = @event.UniqueName.Value;
    }
    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }

    if (@event.BaseFriendship is not null)
    {
      BaseFriendship = @event.BaseFriendship.Value;
    }
    if (@event.CatchRate is not null)
    {
      CatchRate = @event.CatchRate.Value;
    }
    if (@event.GrowthRate.HasValue)
    {
      GrowthRate = @event.GrowthRate.Value;
    }

    if (@event.Link is not null)
    {
      Link = @event.Link.Value?.Value;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
