using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Microsoft.Extensions.Configuration;
using PokeCraft.Application.Accounts;

namespace PokeCraft.Infrastructure.Identity;

internal class RealmService : IRealmService
{
  private readonly IRealmClient _realmClient;
  private readonly string _uniqueSlug;

  public RealmService(IConfiguration configuration, IRealmClient realmClient)
  {
    _realmClient = realmClient;

    string? uniqueSlug = Environment.GetEnvironmentVariable("PORTAL_REALM");
    if (!string.IsNullOrWhiteSpace(uniqueSlug))
    {
      _uniqueSlug = uniqueSlug;
    }
    _uniqueSlug = configuration.GetSection("Portal").GetValue<string?>("Realm")
      ?? throw new ArgumentException("The configuration key 'Portal:Realm' is required.", nameof(configuration));
  }

  public async Task<RealmModel> FindAsync(CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _realmClient.ReadAsync(id: null, _uniqueSlug, context) ?? throw new InvalidOperationException($"The realm 'UniqueSlug={_uniqueSlug}' could not be found.");
  }
}
