using Logitar.Portal.Contracts.Realms;

namespace PokeCraft.Application.Accounts;

public interface IRealmService
{
  Task<RealmModel> FindAsync(CancellationToken cancellationToken = default);
}
