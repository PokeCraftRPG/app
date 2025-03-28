using FluentValidation;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Speciez.Validators;

internal class CreateOrReplaceSpeciesValidator : AbstractValidator<CreateOrReplaceSpeciesPayload>
{
  public CreateOrReplaceSpeciesValidator()
  {
    RuleFor(x => x.Number).SpeciesNumber();
    RuleFor(x => x.Category).IsInEnum();

    RuleFor(x => x.UniqueName).UniqueName();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());

    RuleFor(x => x.BaseFriendship).Friendship();
    RuleFor(x => x.CatchRate).CatchRate();
    RuleFor(x => x.GrowthRate).IsInEnum();

    RuleForEach(x => x.RegionalNumbers).SetValidator(new RegionalNumberValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Link), () => RuleFor(x => x.Link!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
