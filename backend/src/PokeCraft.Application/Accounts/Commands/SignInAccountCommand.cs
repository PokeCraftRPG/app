using FluentValidation;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;
using PokeCraft.Application.Accounts.Models;
using PokeCraft.Application.Accounts.Validators;
using PokeCraft.Application.Constants;

namespace PokeCraft.Application.Accounts.Commands;

/// <exception cref="InvalidOneTimePasswordPurposeException"></exception>
/// <exception cref="OneTimePasswordNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public record SignInAccountCommand(SignInAccountPayload Payload, IEnumerable<CustomAttribute> CustomAttributes) : IRequest<SignInAccountResult>;

internal class SignInAccountCommandHandler : IRequestHandler<SignInAccountCommand, SignInAccountResult>
{
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IRealmService _realmService;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public SignInAccountCommandHandler(
    IMessageService messageService,
    IOneTimePasswordService oneTimePasswordService,
    IRealmService realmService,
    ISessionService sessionService,
    ITokenService tokenService,
    IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _realmService = realmService;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<SignInAccountResult> Handle(SignInAccountCommand command, CancellationToken cancellationToken)
  {
    RealmModel realm = await _realmService.FindAsync(cancellationToken);

    SignInAccountPayload payload = command.Payload;
    new SignInAccountValidator(realm.PasswordSettings).ValidateAndThrow(payload);

    if (payload.Credentials is not null)
    {
      return await HandleCredentialsAsync(payload.Credentials, payload.Locale, command.CustomAttributes, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(payload.AuthenticationToken))
    {
      return await HandleAuthenticationTokenAsync(payload.AuthenticationToken, command.CustomAttributes, cancellationToken);
    }
    else if (payload.OneTimePassword is not null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, command.CustomAttributes, cancellationToken);
    }
    else if (payload.Profile is not null)
    {
      return await HandleProfileAsync(payload.Profile, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"Exactly one of the following must be specified: {nameof(payload.Credentials)}, {nameof(payload.AuthenticationToken)}, {nameof(payload.OneTimePassword)}, {nameof(payload.Profile)}.", nameof(command));
  }

  private async Task<SignInAccountResult> HandleCredentialsAsync(Credentials credentials, string locale, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    UserModel? user = await _userService.FindAsync(credentials.EmailAddress, cancellationToken);
    if (user is null || !user.HasPassword)
    {
      EmailModel email = user?.Email ?? new(credentials.EmailAddress);
      CreatedTokenModel authentication = await _tokenService.CreateAsync(user, email, TokenTypes.Authentication, TokenLifetimes.Authentication, isConsumable: true, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        [Variables.Token] = authentication.Token
      };
      SentMessages sentMessages = user is null
        ? await _messageService.SendAsync(Templates.AccountAuthentication, email, locale, variables, cancellationToken)
        : await _messageService.SendAsync(Templates.AccountAuthentication, user, ContactType.Email, locale, variables, cancellationToken);
      SentMessage sentMessage = sentMessages.ToSentMessage(email);
      return SignInAccountResult.AuthenticationLinkSent(sentMessage);
    }
    else if (string.IsNullOrWhiteSpace(credentials.Password))
    {
      return SignInAccountResult.RequirePassword();
    }

    MultiFactorAuthenticationMode? multiFactorAuthenticationMode = user.GetMultiFactorAuthenticationMode();
    if (multiFactorAuthenticationMode == MultiFactorAuthenticationMode.None && user.IsProfileCompleted())
    {
      SessionModel session = await _sessionService.SignInAsync(user, credentials.Password, customAttributes, cancellationToken);
      return SignInAccountResult.Success(session);
    }

    user = await _userService.AuthenticateAsync(user, credentials.Password, cancellationToken);

    return multiFactorAuthenticationMode switch
    {
      MultiFactorAuthenticationMode.Email => await SendMultiFactorAuthenticationMessageAsync(user, ContactType.Email, locale, cancellationToken),
      MultiFactorAuthenticationMode.Phone => await SendMultiFactorAuthenticationMessageAsync(user, ContactType.Phone, locale, cancellationToken),
      _ => await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken),
    };
  }
  private async Task<SignInAccountResult> SendMultiFactorAuthenticationMessageAsync(UserModel user, ContactType contactType, string locale, CancellationToken cancellationToken)
  {
    ContactModel contact = contactType switch
    {
      ContactType.Email => user.Email ?? throw new ArgumentException($"The user 'Id={user.Id}' has no email.", nameof(user)),
      ContactType.Phone => user.Phone ?? throw new ArgumentException($"The user 'Id={user.Id}' has no phone.", nameof(user)),
      _ => throw new ArgumentException($"The contact type '{contactType}' is not supported.", nameof(contactType)),
    };
    OneTimePasswordModel oneTimePassword = await _oneTimePasswordService.CreateAsync(user, Purposes.MultiFactorAuthentication, cancellationToken);
    if (oneTimePassword.Password is null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      [Variables.OneTimePassword] = oneTimePassword.Password
    };
    string template = Templates.GetMultiFactorAuthentication(contactType);
    SentMessages sentMessages = await _messageService.SendAsync(template, user, contactType, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(contact);
    return SignInAccountResult.RequireOneTimePasswordValidation(oneTimePassword, sentMessage);
  }

  private async Task<SignInAccountResult> HandleAuthenticationTokenAsync(string authenticationToken, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedTokenModel validatedToken = await _tokenService.ValidateAsync(authenticationToken, TokenTypes.Authentication, consume: true, cancellationToken);
    EmailModel email = validatedToken.Email ?? throw new ArgumentException("The email claims are required.", nameof(authenticationToken));

    UserModel user;
    if (string.IsNullOrWhiteSpace(validatedToken.Subject))
    {
      user = await _userService.CreateAsync(email.Address, cancellationToken);
    }
    else
    {
      Guid userId = validatedToken.GetUserId();
      user = await _userService.FindAsync(userId, cancellationToken);
      if (user.Email is null || !user.Email.IsVerified)
      {
        EmailPayload newEmail = new(email.Address, isVerified: true);
        user = await _userService.UpdateAsync(user, newEmail, cancellationToken);
      }
    }

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInAccountResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    OneTimePasswordModel oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.MultiFactorAuthentication, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    UserModel user = await _userService.FindAsync(userId, cancellationToken);

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInAccountResult> HandleProfileAsync(CompleteProfilePayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedTokenModel validatedToken = await _tokenService.ValidateAsync(payload.Token, TokenTypes.Profile, consume: true, cancellationToken);
    Guid userId = validatedToken.GetUserId();
    UserModel user = await _userService.FindAsync(userId, cancellationToken);
    user = await _userService.CompleteProfileAsync(user, payload, cancellationToken);

    return await EnsureProfileIsCompletedAsync(user, customAttributes, cancellationToken);
  }

  private async Task<SignInAccountResult> EnsureProfileIsCompletedAsync(UserModel user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      EmailModel email = user.Email ?? throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
      CreatedTokenModel profileCompletion = await _tokenService.CreateAsync(user, email, TokenTypes.Profile, TokenLifetimes.Profile, isConsumable: true, cancellationToken);
      return SignInAccountResult.RequireProfileCompletion(profileCompletion);
    }

    SessionModel session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    return SignInAccountResult.Success(session);
  }
}
