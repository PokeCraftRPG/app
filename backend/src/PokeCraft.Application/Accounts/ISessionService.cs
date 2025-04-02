using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace PokeCraft.Application.Accounts;

public interface ISessionService
{
  Task<SessionModel> CreateAsync(UserModel user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken = default);
  Task<SessionModel> RenewAsync(string refreshToken, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken = default);
  Task<SessionModel> SignInAsync(UserModel user, string password, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken = default);
}
