using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Models.Account;

namespace PokeCraft.Authentication;

public interface IOpenAuthenticationService
{
  Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken = default);
  Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken = default);

  Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken = default);
  Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);
}

internal class OpenAuthenticationService : IOpenAuthenticationService // TODO(fpion): OAuth
{
  public Task<TokenResponse> GetTokenResponseAsync(SessionModel session, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public Task<TokenResponse> GetTokenResponseAsync(UserModel user, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<SessionModel> GetSessionAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public Task<UserModel> GetUserAsync(string accessToken, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
