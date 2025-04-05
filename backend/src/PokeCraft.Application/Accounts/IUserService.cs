using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Application.Accounts;

public interface IUserService
{
  Task<UserModel> AuthenticateAsync(string emailAddress, string password, CancellationToken cancellationToken = default);
  Task<UserModel> AuthenticateAsync(UserModel user, string password, CancellationToken cancellationToken = default);
  Task<UserModel> CompleteProfileAsync(UserModel user, CompleteProfilePayload payload, CancellationToken cancellationToken = default);
  Task<UserModel> CreateAsync(string emailAddress, CancellationToken cancellationToken = default);
  Task<UserModel?> FindAsync(string emailAddress, CancellationToken cancellationToken = default);
  Task<UserModel> FindAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<UserModel>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
  Task<UserModel> UpdateAsync(UserModel user, EmailPayload email, CancellationToken cancellationToken = default);
}
