using Logitar.Portal.Contracts.Tokens;

namespace PokeCraft.Application.Accounts;

public static class TokenExtensions
{
  public static Guid GetUserId(this ValidatedTokenModel token)
  {
    if (token.Subject is null)
    {
      throw new ArgumentException($"The {nameof(token.Subject)} is required.", nameof(token));
    }
    return Guid.Parse(token.Subject);
  }
}
