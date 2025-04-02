using PokeCraft.Application.Accounts.Models;

namespace PokeCraft.Models.Account;

public record GetTokenResponse
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public TokenResponse? TokenResponse { get; set; }

  public GetTokenResponse()
  {
  }

  public GetTokenResponse(SignInAccountResult result)
  {
    AuthenticationLinkSentTo = result.AuthenticationLinkSentTo;
    IsPasswordRequired = result.IsPasswordRequired;
    OneTimePasswordValidation = result.OneTimePasswordValidation;
    ProfileCompletionToken = result.ProfileCompletionToken;
  }
}
