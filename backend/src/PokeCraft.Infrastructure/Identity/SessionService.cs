using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts;

namespace PokeCraft.Infrastructure.Identity;

internal class SessionService : ISessionService
{
  private readonly ISessionClient _sessionClient;

  public SessionService(ISessionClient sessionClient)
  {
    _sessionClient = sessionClient;
  }

  public async Task<SessionModel> CreateAsync(UserModel user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = new(user.UniqueName, isPersistent: true, customAttributes);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _sessionClient.CreateAsync(payload, context);
  }

  public async Task<SessionModel> RenewAsync(string refreshToken, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = new(refreshToken, customAttributes);
    RequestContext context = new(cancellationToken);
    return await _sessionClient.RenewAsync(payload, context);
  }

  public async Task<SessionModel> SignInAsync(UserModel user, string password, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(user.UniqueName, password, isPersistent: true, customAttributes);
    RequestContext context = new(user.Id.ToString(), cancellationToken);
    return await _sessionClient.SignInAsync(payload, context);
  }
}
