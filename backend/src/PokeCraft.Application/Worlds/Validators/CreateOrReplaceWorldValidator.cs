using FluentValidation;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Worlds.Validators;

internal class CreateOrReplaceWorldValidator : AbstractValidator<CreateOrReplaceWorldPayload>
{
  public CreateOrReplaceWorldValidator()
  {
    RuleFor(x => x.UniqueSlug).Slug();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
