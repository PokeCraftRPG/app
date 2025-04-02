using Logitar.Portal.Contracts.Sessions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Application.Accounts;
using PokeCraft.Application.Accounts.Commands;
using PokeCraft.Application.Accounts.Models;
using PokeCraft.Authentication;
using PokeCraft.Extensions;
using PokeCraft.Models.Account;

namespace PokeCraft.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IOpenAuthenticationService _openAuthenticationService;
  private readonly ISessionService _sessionService;

  public AccountController(IMediator mediator, IOpenAuthenticationService openAuthenticationService, ISessionService sessionService)
  {
    _mediator = mediator;
    _openAuthenticationService = openAuthenticationService;
    _sessionService = sessionService;
  }

  [HttpPost("/auth/sign/in")]
  public async Task<ActionResult<SignInAccountResponse>> SignInAsync([FromBody] SignInAccountPayload payload, CancellationToken cancellationToken)
  {
    SignInAccountCommand command = new(payload, HttpContext.GetSessionCustomAttributes());
    SignInAccountResult result = await _mediator.Send(command, cancellationToken);
    if (result.Session is not null)
    {
      HttpContext.SignIn(result.Session);
    }

    SignInAccountResponse response = new(result);
    return Ok(response);
  }

  [HttpPost("/auth/token")]
  public async Task<ActionResult<GetTokenResponse>> GetTokenAsync([FromBody] GetTokenPayload payload, CancellationToken cancellationToken)
  {
    GetTokenResponse response;
    SessionModel? session;
    if (!string.IsNullOrWhiteSpace(payload.RefreshToken))
    {
      response = new GetTokenResponse();
      session = await _sessionService.RenewAsync(payload.RefreshToken.Trim(), HttpContext.GetSessionCustomAttributes(), cancellationToken);
    }
    else
    {
      SignInAccountCommand command = new(payload, HttpContext.GetSessionCustomAttributes());
      SignInAccountResult result = await _mediator.Send(command, cancellationToken);
      response = new(result);
      session = result.Session;
    }

    if (session is not null)
    {
      response.TokenResponse = await _openAuthenticationService.GetTokenResponseAsync(session, cancellationToken);
    }

    return Ok(response);
  }
}
