﻿using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PokeCraft.Application.Accounts;
using PokeCraft.Constants;
using PokeCraft.Extensions;

namespace PokeCraft.Authentication;

internal class BasicAuthenticationOptions : AuthenticationSchemeOptions;

internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
  private readonly IUserService _userService;

  public BasicAuthenticationHandler(IUserService userService, IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _userService = userService;
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (Context.Request.Headers.TryGetValue(Headers.Authorization, out StringValues authorization))
    {
      string? value = authorization.Single();
      if (!string.IsNullOrWhiteSpace(value))
      {
        string[] values = value.Split();
        if (values.Length != 2)
        {
          return AuthenticateResult.Fail($"The Authorization header value is not valid: '{value}'.");
        }
        else if (values[0] == Schemes.Basic)
        {
          byte[] bytes = Convert.FromBase64String(values[1]);
          string credentials = Encoding.UTF8.GetString(bytes);
          int index = credentials.IndexOf(':');
          if (index <= 0)
          {
            return AuthenticateResult.Fail($"The Basic credentials are not valid: '{credentials}'.");
          }
          string username = credentials[..index];
          string password = credentials[(index + 1)..];

          try
          {
            UserModel user = await _userService.AuthenticateAsync(username, password);
            MultiFactorAuthenticationMode multiFactorAuthenticationMode = user.GetMultiFactorAuthenticationMode();
            if (multiFactorAuthenticationMode != MultiFactorAuthenticationMode.None)
            {
              return AuthenticateResult.Fail($"The user 'Id={user.Id}' Multi-Factor Authentication mode '{multiFactorAuthenticationMode}' is not '{MultiFactorAuthenticationMode.None}'.");
            }
            else if (!user.IsProfileCompleted())
            {
              return AuthenticateResult.Fail($"The user 'Id={user.Id}' profile is not completed.");
            }

            Context.SetUser(user);

            ClaimsPrincipal principal = new(user.CreateClaimsIdentity(Scheme.Name));
            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
          }
          catch (Exception exception)
          {
            return AuthenticateResult.Fail(exception);
          }
        }
      }
    }

    return AuthenticateResult.NoResult();
  }
}
