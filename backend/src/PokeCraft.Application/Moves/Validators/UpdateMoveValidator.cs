using FluentValidation;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves.Validators;

internal class UpdateMoveValidator : AbstractValidator<UpdateMovePayload>
{
  public UpdateMoveValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Accuracy?.Value is not null, () => RuleFor(x => x.Accuracy!.Value!.Value).Accuracy());
    When(x => x.Power?.Value is not null, () => RuleFor(x => x.Power!.Value!.Value).Power());
    When(x => x.PowerPoints.HasValue, () => RuleFor(x => x.PowerPoints!.Value).PowerPoints());

    When(x => x.InflictedStatus?.Value is not null, () => RuleFor(x => x.InflictedStatus!.Value!).SetValidator(new InflictedStatusValidator()));
    RuleForEach(x => x.StatisticChanges).SetValidator(new StatisticChangeValidator());
    RuleForEach(x => x.VolatileConditions).SetValidator(new VolatileConditionActionValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Link?.Value), () => RuleFor(x => x.Link!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
  }
}
