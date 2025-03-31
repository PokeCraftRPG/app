using Logitar;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez;

public class RegionalNumberAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified regional number is already used.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid SpeciesId
  {
    get => (Guid)Data[nameof(SpeciesId)]!;
    private set => Data[nameof(SpeciesId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public Guid RegionId
  {
    get => (Guid)Data[nameof(RegionId)]!;
    private set => Data[nameof(RegionId)] = value;
  }
  public int Number
  {
    get => (int)Data[nameof(Number)]!;
    private set => Data[nameof(Number)] = value;
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
      error.Data[nameof(SpeciesId)] = SpeciesId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(RegionId)] = RegionId;
      error.Data[nameof(Number)] = Number;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RegionalNumberAlreadyUsedException(Species species, SpeciesId conflictId, RegionId regionId) : base(BuildMessage(species, conflictId, regionId))
  {
    WorldId = species.WorldId.ToGuid();
    SpeciesId = species.EntityId;
    ConflictId = conflictId.EntityId;
    RegionId = regionId.EntityId;
    Number = species.RegionalNumbers[regionId].Value;
    PropertyName = nameof(species.Number);
  }

  private static string BuildMessage(Species species, SpeciesId conflictId, RegionId regionId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), species.WorldId.ToGuid())
    .AddData(nameof(SpeciesId), species.EntityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(RegionId), regionId.EntityId)
    .AddData(nameof(Number), species.RegionalNumbers[regionId].Value)
    .AddData(nameof(PropertyName), nameof(species.Number))
    .Build();
}
