﻿using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using PokeCraft.Constants;
using PokeCraft.Extensions;

namespace PokeCraft.Authentication;

internal class BearerAuthenticationOptions : AuthenticationSchemeOptions;

internal class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
  private readonly IOpenAuthenticationService _openAuthenticationService;

  public BearerAuthenticationHandler(IOpenAuthenticationService openAuthenticationService, IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : base(options, logger, encoder)
  {
    _openAuthenticationService = openAuthenticationService;
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
        else if (values[0] == Schemes.Bearer)
        {
          try
          {
            SessionModel session = await _openAuthenticationService.GetSessionAsync(values[1]);
            UserModel user = session.User;

            Context.SetSession(session);
            Context.SetUser(session.User);

            ClaimsPrincipal principal = new(session.CreateClaimsIdentity(Scheme.Name));
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
