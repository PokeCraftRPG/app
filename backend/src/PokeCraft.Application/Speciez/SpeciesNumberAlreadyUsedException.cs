using Logitar;
using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez;

public class SpeciesNumberAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified species number is already used.";

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
      error.Data[nameof(Number)] = Number;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public SpeciesNumberAlreadyUsedException(Species species, SpeciesId conflictId) : base(BuildMessage(species, conflictId))
  {
    WorldId = species.WorldId.ToGuid();
    SpeciesId = species.EntityId;
    ConflictId = conflictId.EntityId;
    Number = species.Number.Value;
    PropertyName = nameof(species.Number);
  }

  private static string BuildMessage(Species species, SpeciesId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), species.WorldId.ToGuid())
    .AddData(nameof(SpeciesId), species.EntityId)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(Number), species.Number.Value)
    .AddData(nameof(PropertyName), nameof(species.Number))
    .Build();
}
