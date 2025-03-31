using Logitar;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<ActorId, ActorModel> _actors = [];
  private readonly ActorModel _system = ActorModel.System;

  public Mapper()
  {
  }

  public Mapper(IEnumerable<ActorModel> actors)
  {
    foreach (ActorModel actor in actors)
    {
      ActorId id = new(actor.Id);
      _actors[id] = actor;
    }
  }

  public AbilityModel ToAbility(AbilityEntity source)
  {
    if (source.World is null)
    {
      throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source));
    }
    WorldModel world = ToWorld(source.World);
    return ToAbility(source, world);
  }
  public AbilityModel ToAbility(AbilityEntity source, WorldModel world)
  {
    AbilityModel destination = new()
    {
      Id = source.Id,
      World = world,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Link = source.Link,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  public MoveModel ToMove(MoveEntity source)
  {
    if (source.World is null)
    {
      throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source));
    }
    WorldModel world = ToWorld(source.World);
    return ToMove(source, world);
  }
  public MoveModel ToMove(MoveEntity source, WorldModel world)
  {
    MoveModel destination = new()
    {
      Id = source.Id,
      World = world,
      Type = source.Type,
      Category = source.Category,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Accuracy = source.Accuracy,
      Power = source.Power,
      PowerPoints = source.PowerPoints,
      InflictedStatus = source.GetInflictedStatus(),
      Link = source.Link,
      Notes = source.Notes
    };
    destination.StatisticChanges.AddRange(source.GetStatisticChanges().Select(x => new StatisticChangeModel(x)));
    destination.VolatileConditions.AddRange(source.GetVolatileConditions());

    MapAggregate(source, destination);

    return destination;
  }

  public RegionModel ToRegion(RegionEntity source)
  {
    if (source.World is null)
    {
      throw new ArgumentException($"The {nameof(source.World)} is required.", nameof(source));
    }
    WorldModel world = ToWorld(source.World);
    return ToRegion(source, world);
  }
  public RegionModel ToRegion(RegionEntity source, WorldModel world)
  {
    RegionModel destination = new()
    {
      Id = source.Id,
      World = world,
      UniqueName = source.UniqueName,
      DisplayName = source.DisplayName,
      Description = source.Description,
      Link = source.Link,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(WorldEntity source)
  {
    WorldModel destination = new()
    {
      Id = source.Id,
      UniqueSlug = source.UniqueSlug,
      DisplayName = source.DisplayName,
      Description = source.Description
    };

    ActorId ownerId = new(source.OwnerId);
    destination.Owner = _actors.TryGetValue(ownerId, out ActorModel? owner) ? owner : _system;

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, AggregateModel destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private ActorModel? TryFindActor(string? id) => id is null ? null : TryFindActor(new ActorId(id));
  private ActorModel? TryFindActor(ActorId? id) => id.HasValue && _actors.TryGetValue(id.Value, out ActorModel? actor) ? actor : null;
}
