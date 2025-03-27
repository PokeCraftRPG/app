using FluentValidation;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Abilities.Validators;

internal class CreateOrReplaceAbilityValidator : AbstractValidator<CreateOrReplaceAbilityPayload>
{
  public CreateOrReplaceAbilityValidator()
  {
    RuleFor(x => x.UniqueName).UniqueName();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Link), () => RuleFor(x => x.Link!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
  }
}
