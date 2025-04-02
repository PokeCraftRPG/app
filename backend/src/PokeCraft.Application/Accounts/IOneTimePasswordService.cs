using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Application.Accounts;

public interface IOneTimePasswordService
{
  Task<OneTimePasswordModel> CreateAsync(UserModel user, string purpose, CancellationToken cancellationToken = default);
  Task<OneTimePasswordModel> ValidateAsync(OneTimePasswordPayload payload, string purpose, CancellationToken cancellationToken = default);
}
