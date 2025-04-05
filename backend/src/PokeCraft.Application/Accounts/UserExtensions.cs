using Logitar;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Accounts.Models;
using PokeCraft.Application.Constants;

namespace PokeCraft.Application.Accounts;

public static class UserExtensions
{
  public static void CompleteProfile(this UpdateUserPayload payload)
  {
    CustomAttributeModification customAttribute = new(CustomAttributes.ProfileCompletedOn, DateTime.Now.ToISOString());
    payload.CustomAttributes.Add(customAttribute);
  }

  public static MultiFactorAuthenticationMode GetMultiFactorAuthenticationMode(this UserModel user)
  {
    string? value = user.CustomAttributes.SingleOrDefault(x => x.Key == CustomAttributes.MultiFactorAuthenticationMode)?.Value;
    return string.IsNullOrWhiteSpace(value) ? MultiFactorAuthenticationMode.None : Enum.Parse<MultiFactorAuthenticationMode>(value);
  }

  public static string GetSubject(this UserModel user) => user.Id.ToString();

  public static UserType GetUserType(this UserModel user)
  {
    CustomAttribute? customAttribute = user.GetCustomAttribute(CustomAttributes.UserType);
    return customAttribute is null ? UserType.Player : Enum.Parse<UserType>(customAttribute.Value);
  }

  public static bool IsProfileCompleted(this UserModel user) => user.GetCustomAttribute(CustomAttributes.ProfileCompletedOn) is not null;

  public static void SetMultiFactorAuthenticationMode(this UpdateUserPayload payload, MultiFactorAuthenticationMode multiFactorAuthenticationMode)
  {
    CustomAttributeModification customAttribute = new(CustomAttributes.MultiFactorAuthenticationMode, multiFactorAuthenticationMode.ToString());
    payload.CustomAttributes.Add(customAttribute);
  }

  public static void SetUserType(this UpdateUserPayload payload, UserType userType)
  {
    CustomAttributeModification customAttribute = new(CustomAttributes.UserType, userType.ToString());
    payload.CustomAttributes.Add(customAttribute);
  }

  public static UpdateUserPayload ToUpdateUserPayload(this SaveProfilePayload payload)
  {
    UpdateUserPayload updatePayload = new()
    {
      FirstName = new ChangeModel<string>(payload.FirstName),
      MiddleName = new ChangeModel<string>(payload.MiddleName),
      LastName = new ChangeModel<string>(payload.LastName),
      Birthdate = new ChangeModel<DateTime?>(payload.Birthdate),
      Gender = new ChangeModel<string>(payload.Gender),
      Locale = new ChangeModel<string>(payload.Locale),
      TimeZone = new ChangeModel<string>(payload.TimeZone),
      Picture = new ChangeModel<string>(payload.Picture)
    };
    updatePayload.SetMultiFactorAuthenticationMode(payload.MultiFactorAuthenticationMode);
    updatePayload.SetUserType(payload.UserType);
    return updatePayload;
  }

  private static CustomAttribute? GetCustomAttribute(this UserModel user, string key)
  {
    CustomAttribute[] customAttributes = user.CustomAttributes.Where(x => x.Key == key).ToArray();
    if (customAttributes.Length > 1)
    {
      throw new ArgumentException($"Multiple ({customAttributes.Length}) custom attributes '{key}' were found for user (Id={user.Id}).", nameof(user));
    }
    return customAttributes.SingleOrDefault();
  }
}
