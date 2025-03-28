using FluentValidation;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Speciez.Validators;

internal class RegionalNumberUpdateValidator : AbstractValidator<RegionalNumberUpdatePayload>
{
  public RegionalNumberUpdateValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Region) && x.Number.HasValue, () => RuleFor(x => x.Number!.Value).SpeciesNumber());
  }
}
