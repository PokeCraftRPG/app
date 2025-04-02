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
