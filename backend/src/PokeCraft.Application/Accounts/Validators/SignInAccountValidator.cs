using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Application.Accounts.Validators;

internal class SignInAccountValidator : AbstractValidator<SignInAccountPayload>
{
  public SignInAccountValidator(IPasswordSettings passwordSettings)
  {
    //RuleFor(x => x.Locale).Locale(); // TODO(fpion): validation

    When(x => x.Credentials != null, () => RuleFor(x => x.Credentials!).SetValidator(new CredentialsValidator()));
    When(x => x.OneTimePassword != null, () => RuleFor(x => x.OneTimePassword!).SetValidator(new OneTimePasswordValidator()));
    When(x => x.Profile != null, () => RuleFor(x => x.Profile!).SetValidator(new CompleteProfileValidator(passwordSettings)));

    RuleFor(x => x).Must(BeAValidPayload).WithErrorCode(nameof(SignInAccountValidator))
      .WithMessage(x => $"Exactly one of the following must be specified: {nameof(x.Credentials)}, {nameof(x.AuthenticationToken)}, {nameof(x.OneTimePassword)}, {nameof(x.Profile)}.");
  }

  private static bool BeAValidPayload(SignInAccountPayload payload)
  {
    int count = 0;
    if (payload.Credentials != null)
    {
      count++;
    }
    if (!string.IsNullOrWhiteSpace(payload.AuthenticationToken))
    {
      count++;
    }
    if (payload.OneTimePassword != null)
    {
      count++;
    }
    if (payload.Profile != null)
    {
      count++;
    }
    return count == 1;
  }
}
