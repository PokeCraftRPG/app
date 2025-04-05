using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace PokeCraft.Application.Accounts;

public interface ITokenService
{
  Task<CreatedTokenModel> CreateAsync(UserModel? user, IEmail email, string type, int lifetimeSeconds, bool isConsumable, CancellationToken cancellationToken = default);
  Task<CreatedTokenModel> CreateAsync(UserModel user, IEnumerable<Claim> claims, string type, int lifetimeSeconds, bool isConsumable, CancellationToken cancellationToken = default);
  Task<ValidatedTokenModel> ValidateAsync(string token, string type, bool consume, CancellationToken cancellationToken = default);
}
