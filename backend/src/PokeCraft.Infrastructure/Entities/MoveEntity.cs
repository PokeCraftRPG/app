using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;
using PokeCraft.Domain.Moves.Events;
using PokeCraft.Infrastructure.Converters;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Entities;

internal class MoveEntity : AggregateEntity, ISegregatedEntity
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static MoveEntity()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    _serializerOptions.Converters.Add(new VolatileConditionConverter());
  }

  public int MoveId { get; private set; }

  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public Guid Id { get; private set; }

  public PokemonType Type { get; private set; }
  public MoveCategory Category { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int? Accuracy { get; private set; }
  public int? Power { get; private set; }
  public int PowerPoints { get; private set; }

  public StatusCondition? StatusCondition { get; private set; }
  public int? StatusChance { get; private set; }
  public string? StatisticChanges { get; private set; }
  public string? VolatileConditions { get; private set; }

  public string? Link { get; private set; }
  public string? Notes { get; private set; }

  public MoveEntity(WorldEntity world, MoveCreated @event) : base(@event)
  {
    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    Id = new MoveId(@event.StreamId).EntityId;

    Type = @event.Type;
    Category = @event.Category;

    UniqueName = @event.UniqueName.Value;

    Power = @event.PowerPoints.Value;
  }

  private MoveEntity() : base()
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

  public void Update(MoveUpdated @event)
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

    if (@event.Accuracy is not null)
    {
      Accuracy = @event.Accuracy.Value?.Value;
    }
    if (@event.Power is not null)
    {
      Power = @event.Power.Value?.Value;
    }
    if (@event.PowerPoints is not null)
    {
      PowerPoints = @event.PowerPoints.Value;
    }

    if (@event.InflictedStatus is not null)
    {
      StatusCondition = @event.InflictedStatus.Value?.Condition;
      StatusChance = @event.InflictedStatus.Value?.Chance;
    }
    if (@event.StatisticChanges.Count > 0)
    {
      Dictionary<PokemonStatistic, int> statisticChanges = GetStatisticChanges();
      foreach (KeyValuePair<PokemonStatistic, int> change in @event.StatisticChanges)
      {
        if (change.Value == 0)
        {
          statisticChanges.Remove(change.Key);
        }
        else
        {
          statisticChanges[change.Key] = change.Value;
        }
      }
      StatisticChanges = statisticChanges.Count < 1 ? null : JsonSerializer.Serialize(statisticChanges, _serializerOptions);
    }
    if (@event.VolatileConditions is not null)
    {
      VolatileConditions = @event.VolatileConditions.Count < 1 ? null : JsonSerializer.Serialize(@event.VolatileConditions, _serializerOptions);
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

  public InflictedStatusModel? GetInflictedStatus()
  {
    if (StatusCondition.HasValue && StatusChance.HasValue)
    {
      return new InflictedStatusModel(StatusCondition.Value, StatusChance.Value);
    }

    return null;
  }
  public Dictionary<PokemonStatistic, int> GetStatisticChanges()
  {
    return (StatisticChanges is null ? null : JsonSerializer.Deserialize<Dictionary<PokemonStatistic, int>>(StatisticChanges, _serializerOptions)) ?? [];
  }
  public IReadOnlyCollection<string> GetVolatileConditions()
  {
    return (VolatileConditions is null ? null : JsonSerializer.Deserialize<IReadOnlyCollection<string>>(VolatileConditions, _serializerOptions)) ?? [];
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
