using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts;

namespace PokeCraft.Infrastructure.Identity;

internal class TokenService : ITokenService
{
  private readonly ITokenClient _tokenClient;

  public TokenService(ITokenClient tokenClient)
  {
    _tokenClient = tokenClient;
  }

  public async Task<CreatedTokenModel> CreateAsync(UserModel? user, IEmail email, string type, int lifetimeSeconds, bool isConsumable, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = new()
    {
      IsConsumable = isConsumable,
      LifetimeSeconds = lifetimeSeconds,
      Type = type,
      Subject = user?.GetSubject(),
      Email = new EmailPayload(email.Address, email.IsVerified)
    };
    RequestContext context = new(cancellationToken);
    return await _tokenClient.CreateAsync(payload, context);
  }

  public async Task<CreatedTokenModel> CreateAsync(UserModel user, IEnumerable<Claim> claims, string type, int lifetimeSeconds, bool isConsumable, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = new()
    {
      IsConsumable = isConsumable,
      LifetimeSeconds = lifetimeSeconds,
      Type = type,
      Subject = user.GetSubject()
    };
    foreach (Claim claim in claims)
    {
      payload.Claims.Add(new ClaimModel(claim.Type, claim.Value, claim.ValueType));
    }
    RequestContext context = new(cancellationToken);
    return await _tokenClient.CreateAsync(payload, context);
  }

  public async Task<ValidatedTokenModel> ValidateAsync(string token, string type, bool consume, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = new(token)
    {
      Consume = consume,
      Type = type
    };
    RequestContext context = new(cancellationToken);
    return await _tokenClient.ValidateAsync(payload, context);
  }
}
