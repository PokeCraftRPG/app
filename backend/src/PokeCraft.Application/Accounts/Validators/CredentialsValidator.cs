using FluentValidation;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Application.Accounts.Validators;

internal class CredentialsValidator : AbstractValidator<Credentials>
{
  public CredentialsValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty().MaximumLength(byte.MaxValue).EmailAddress();
  }
}
