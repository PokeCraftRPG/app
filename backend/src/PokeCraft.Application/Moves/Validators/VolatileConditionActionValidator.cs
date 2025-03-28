using FluentValidation;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Moves.Validators;

internal class VolatileConditionActionValidator : AbstractValidator<VolatileConditionAction>
{
  public VolatileConditionActionValidator()
  {
    RuleFor(x => x.Value).VolatileCondition();
    RuleFor(x => x.Action).IsInEnum();
  }
}
