using FluentValidation;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Speciez.Validators;

internal class RegionalNumberValidator : AbstractValidator<RegionalNumberPayload>
{
  public RegionalNumberValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Region), () => RuleFor(x => x.Number).SpeciesNumber());
  }
}
