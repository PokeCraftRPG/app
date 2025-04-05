using FluentValidation;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Application.Accounts.Validators;

internal class OneTimePasswordValidator : AbstractValidator<OneTimePasswordPayload>
{
  public OneTimePasswordValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Code).NotEmpty();
  }
}
