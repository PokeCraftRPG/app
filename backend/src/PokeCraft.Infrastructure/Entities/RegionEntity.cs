﻿using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Regions.Events;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Entities;

internal class RegionEntity : AggregateEntity, ISegregatedEntity
{
  public int RegionId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? Link { get; private set; }
  public string? Notes { get; private set; }

  public List<RegionalNumberEntity> RegionalNumbers { get; private set; } = [];

  public RegionEntity(WorldEntity world, RegionCreated @event) : base(@event)
  {
    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Id = new RegionId(@event.StreamId).EntityId;

    UniqueName = @event.UniqueName.Value;
  }

  private RegionEntity() : base()
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

  public void Update(RegionUpdated @event)
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
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
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
