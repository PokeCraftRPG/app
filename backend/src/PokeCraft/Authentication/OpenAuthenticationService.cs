using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
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

  public OpenAuthenticationService(OpenAuthenticationSettings settings)
  {
    _settings = settings;
  }

  public async Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken)
  {
    AccessTokenSettings settings = _settings.AccessToken;

    CreatedTokenModel access = new(); // TODO(fpion): implement
    // sub
    // lifetime
    // roles
    // auth_time
    // sid
    /*
     * bool IsConsumable
     * string? Algorithm
     * string? Audience
     * string? Issuer
     * int? LifetimeSeconds
     * string? Secret
     * string? Type
     * string? Subject
     * EmailPayload? Email
     * List<ClaimModel> Claims
     */
    await Task.Delay(1000, cancellationToken); // TODO(fpion): implement

    TokenResponse tokenResponse = new(access.Token, Schemes.Bearer)
    {
      ExpiresIn = settings.LifetimeSeconds,
      RefreshToken = session.RefreshToken
    };
    return tokenResponse;
  }
  public Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): OAuth
  }

  public Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): OAuth
  }
  public Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): OAuth
  }
}
