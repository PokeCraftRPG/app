using Logitar.Portal.Contracts.ApiKeys;

namespace PokeCraft.Application.Accounts;

public interface IApiKeyService
{
  Task<ApiKeyModel> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken = default);
}
