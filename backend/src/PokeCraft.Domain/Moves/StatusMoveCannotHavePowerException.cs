using Logitar;

namespace PokeCraft.Domain.Moves;

public class StatusMoveCannotHavePowerException : DomainException
{
  private static readonly string ErrorMessage = $"Moves in the {MoveCategory.Status} category cannot have power.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid MoveId
  {
    get => (Guid)Data[nameof(MoveId)]!;
    private set => Data[nameof(MoveId)] = value;
  }
  public int Power
  {
    get => (int)Data[nameof(Power)]!;
    private set => Data[nameof(Power)] = value;
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
      error.Data[nameof(MoveId)] = MoveId;
      error.Data[nameof(Power)] = Power;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public StatusMoveCannotHavePowerException(Move move, Power power, string propertyName) : base(BuildMessage(move, power, propertyName))
  {
    WorldId = move.WorldId.ToGuid();
    MoveId = move.EntityId;
    Power = power.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Move move, Power power, string propertyName) => new Logitar.ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), move.WorldId.ToGuid())
    .AddData(nameof(MoveId), move.EntityId)
    .AddData(nameof(Power), power.Value)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
