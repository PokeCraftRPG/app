using FluentValidation;

namespace PokeCraft.Domain.Moves;

public class InflictedStatusValidator : AbstractValidator<IInflictedStatus>
{
  public InflictedStatusValidator()
  {
    RuleFor(x => x.Condition).IsInEnum();
    RuleFor(x => x.Chance).InclusiveBetween(1, 100);
  }
}
