using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts;

namespace PokeCraft.Models.Account;

public record CurrentUser
{
  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public UserType UserType { get; set; }

  public CurrentUser() : this(string.Empty)
  {
  }

  public CurrentUser(string displayName, string? emailAddress = null, string? pictureUrl = null, UserType userType = default)
  {
    DisplayName = displayName;
    EmailAddress = emailAddress;
    PictureUrl = pictureUrl;

    UserType = userType;
  }

  public CurrentUser(SessionModel session) : this(session.User)
  {
  }

  public CurrentUser(UserModel user)
  {
    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;

    UserType = user.GetUserType();
  }
}
