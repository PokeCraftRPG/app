using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;

namespace PokeCraft.Application.Accounts.Models;

public record SignInAccountResult
{
  public SentMessage? AuthenticationLinkSentTo { get; set; }
  public bool IsPasswordRequired { get; set; }
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }
  public SessionModel? Session { get; set; }

  public static SignInAccountResult AuthenticationLinkSent(SentMessage to) => new()
  {
    AuthenticationLinkSentTo = to
  };

  public static SignInAccountResult RequirePassword() => new()
  {
    IsPasswordRequired = true
  };

  public static SignInAccountResult RequireOneTimePasswordValidation(OneTimePasswordModel oneTimePassword, SentMessage sentMessage) => new()
  {
    OneTimePasswordValidation = new OneTimePasswordValidation(oneTimePassword, sentMessage)
  };

  public static SignInAccountResult RequireProfileCompletion(CreatedTokenModel profileCompletion) => new()
  {
    ProfileCompletionToken = profileCompletion.Token
  };

  public static SignInAccountResult Success(SessionModel session) => new()
  {
    Session = session
  };
}
