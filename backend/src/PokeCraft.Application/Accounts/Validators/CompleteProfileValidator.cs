using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Application.Accounts.Validators;

internal class CompleteProfileValidator : SaveProfileValidatorBase<CompleteProfilePayload>
{
  public CompleteProfileValidator(IPasswordSettings passwordSettings) : base()
  {
    RuleFor(x => x.Token).NotEmpty();

    //When(x => !string.IsNullOrWhiteSpace(x.Password), () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(passwordSettings))); // TODO(fpion): validation
  }
}
