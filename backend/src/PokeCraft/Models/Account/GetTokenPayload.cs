using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Models.Account;

public record GetTokenPayload : SignInAccountPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }
}
