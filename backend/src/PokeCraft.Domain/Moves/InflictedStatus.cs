using FluentValidation;

namespace PokeCraft.Domain.Moves;

public record InflictedStatus : IInflictedStatus
{
  public StatusCondition Condition { get; }
  public int Chance { get; }

  public InflictedStatus(StatusCondition condition, int chance)
  {
    Condition = condition;
    Chance = chance;
    new InflictedStatusValidator().ValidateAndThrow(this);
  }

  public static InflictedStatus? TryCreate(IInflictedStatus? status) => status is null ? null : new(status.Condition, status.Chance);
}
