using FluentValidation;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Worlds.Validators;

internal class UpdateWorldValidator : AbstractValidator<UpdateWorldPayload>
{
  public UpdateWorldValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueSlug), () => RuleFor(x => x.UniqueSlug!).Slug());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
