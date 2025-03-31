using Logitar;
using PokeCraft.Domain;
using PokeCraft.Domain.Abilities;
using PokeCraft.Domain.Moves;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Speciez;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public ResourceType ResourceType
  {
    get => (ResourceType)Data[nameof(ResourceType)]!;
    private set => Data[nameof(ResourceType)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(WorldId)] = WorldId;
      error.Data[nameof(ResourceType)] = ResourceType;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(Ability ability, AbilityId conflictId)
    : this(ability.WorldId, ability.ResourceType, ability.EntityId, conflictId.EntityId, ability.UniqueName, nameof(ability.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Move move, MoveId conflictId)
    : this(move.WorldId, move.ResourceType, move.EntityId, conflictId.EntityId, move.UniqueName, nameof(move.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Region region, RegionId conflictId)
    : this(region.WorldId, region.ResourceType, region.EntityId, conflictId.EntityId, region.UniqueName, nameof(region.UniqueName))
  {
  }
  public UniqueNameAlreadyUsedException(Species species, SpeciesId conflictId)
    : this(species.WorldId, species.ResourceType, species.EntityId, conflictId.EntityId, species.UniqueName, nameof(species.UniqueName))
  {
  }
  private UniqueNameAlreadyUsedException(WorldId worldId, ResourceType resourceType, Guid entityId, Guid conflictId, UniqueName uniqueName, string propertyName)
    : base(BuildMessage(worldId, resourceType, entityId, conflictId, uniqueName, propertyName))
  {
    WorldId = worldId.ToGuid();
    ResourceType = resourceType;
    EntityId = entityId;
    ConflictId = conflictId;
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, ResourceType resourceType, Guid entityId, Guid conflictId, UniqueName uniqueName, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), worldId.ToGuid())
    .AddData(nameof(ResourceType), resourceType)
    .AddData(nameof(EntityId), entityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(UniqueName), uniqueName)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
