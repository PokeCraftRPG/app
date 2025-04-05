using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves.Models;

public record InflictedStatusModel : IInflictedStatus
{
  public StatusCondition Condition { get; set; }
  public int Chance { get; set; }

  public InflictedStatusModel()
  {
  }

  public InflictedStatusModel(StatusCondition condition, int chance)
  {
    Condition = condition;
    Chance = chance;
  }

  public InflictedStatusModel(IInflictedStatus status) : this(status.Condition, status.Chance)
  {
  }
}
