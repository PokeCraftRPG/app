using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using PokeCraft.Application.Accounts;

namespace PokeCraft.Infrastructure.Identity;

internal class ApiKeyService : IApiKeyService
{
  private readonly IApiKeyClient _apiKeyClient;

  public ApiKeyService(IApiKeyClient apiKeyClient)
  {
    _apiKeyClient = apiKeyClient;
  }

  public async Task<ApiKeyModel> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken)
  {
    AuthenticateApiKeyPayload payload = new(xApiKey);
    RequestContext context = new(cancellationToken);
    return await _apiKeyClient.AuthenticateAsync(payload, context);
  }
}
