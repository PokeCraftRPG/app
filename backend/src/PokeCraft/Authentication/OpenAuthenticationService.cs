using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using PokeCraft.Application.Accounts;
using PokeCraft.Application.Constants;
using PokeCraft.Constants;
using PokeCraft.Models.Account;
using PokeCraft.Settings;

namespace PokeCraft.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken = default);
  Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken = default);

  Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken = default);
  Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);
}

internal class OpenAuthenticationService : IOpenAuthenticationService
{
  private readonly OpenAuthenticationSettings _settings;
  private readonly ITokenService _tokenService;

  public OpenAuthenticationService(OpenAuthenticationSettings settings, ITokenService tokenService)
  {
    _settings = settings;
    _tokenService = tokenService;
  }

  public async Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken)
  {
    Claim[] claims =
    [
      new(Rfc7519ClaimNames.SessionId, session.Id.ToString()),
      ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, session.CreatedOn)
    ];
    return await GetTokenResponseAsync(session.User, claims, session.RefreshToken, cancellationToken);
  }
  public async Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken)
  {
    return await GetTokenResponseAsync(user, claims: [], refreshToken: null, cancellationToken);
  }
  private async Task<TokenResponse> GetTokenResponseAsync(UserModel user, IEnumerable<Claim> claims, string? refreshToken, CancellationToken cancellationToken)
  {
    AccessTokenSettings settings = _settings.AccessToken;

    if (user.Roles.Count > 0)
    {
      claims = claims.Concat(user.Roles.Select(role => new Claim(Rfc7519ClaimNames.Roles, role.UniqueName)));
    }
    CreatedTokenModel access = await _tokenService.CreateAsync(user, claims, TokenTypes.Access, settings.LifetimeSeconds, isConsumable: false, cancellationToken);

    TokenResponse tokenResponse = new(access.Token, Schemes.Bearer)
    {
      ExpiresIn = settings.LifetimeSeconds,
      RefreshToken = refreshToken
    };
    return tokenResponse;
  }

  public async Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken)
  {
    ValidatedTokenModel validatedToken = await ValidateAsync(accessToken, cancellationToken);
    UserModel user = new()
    {
      Id = validatedToken.GetUserId()
    };

    SessionModel session = new(user);
    foreach (ClaimModel claim in validatedToken.Claims)
    {
      switch (claim.Name)
      {
        case Rfc7519ClaimNames.AuthenticationTime:
          session.CreatedOn = ClaimHelper.ExtractDateTime(new Claim(claim.Name, claim.Value, claim.Type));
          break;
        case Rfc7519ClaimNames.SessionId:
          session.Id = Guid.Parse(claim.Value);
          break;
      }
    }
    if (session.Id == Guid.Empty)
    {
      throw new ArgumentException($"The claims '{Rfc7519ClaimNames.SessionId}' is required.", nameof(accessToken));
    }

    return session;
  }
  public async Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    ValidatedTokenModel validatedToken = await ValidateAsync(accessToken, cancellationToken);
    return new UserModel()
    {
      Id = validatedToken.GetUserId()
    };
  }
  private async Task<ValidatedTokenModel> ValidateAsync(string accessToken, CancellationToken cancellationToken)
  {
    return await _tokenService.ValidateAsync(accessToken, TokenTypes.Access, consume: false, cancellationToken);
  }
}
