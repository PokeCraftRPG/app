using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using PokeCraft.Application.Constants;

namespace PokeCraft.Application.Accounts;

public static class OneTimePasswordExtensions
{
  public static void EnsurePurpose(this OneTimePasswordModel oneTimePassword, string purpose)
  {
    CustomAttribute? customAttribute = oneTimePassword.GetCustomAttribute(CustomAttributes.Purpose);
    if (customAttribute is null || customAttribute.Value != purpose)
    {
      throw new InvalidOneTimePasswordPurposeException(oneTimePassword, purpose);
    }
  }

  public static Guid GetUserId(this OneTimePasswordModel oneTimePassword)
  {
    CustomAttribute customAttribute = oneTimePassword.GetCustomAttribute(CustomAttributes.UserId)
      ?? throw new ArgumentException($"No custom attribute '{CustomAttributes.UserId}' was found for One-Time Password 'Id={oneTimePassword.Id}'.", nameof(oneTimePassword));
    return Guid.Parse(customAttribute.Value);
  }

  public static void SetPurpose(this CreateOneTimePasswordPayload payload, string purpose)
  {
    CustomAttribute customAttribute = new(CustomAttributes.Purpose, purpose);
    payload.CustomAttributes.Add(customAttribute);
  }

  public static void SetUserId(this CreateOneTimePasswordPayload payload, UserModel user)
  {
    CustomAttribute customAttribute = new(CustomAttributes.UserId, user.Id.ToString());
    payload.CustomAttributes.Add(customAttribute);
  }

  public static string? TryGetPurpose(this OneTimePasswordModel oneTimePassword)
  {
    CustomAttribute[] customAttributes = oneTimePassword.CustomAttributes.Where(x => x.Key == CustomAttributes.Purpose).ToArray();
    return customAttributes.Length == 1 ? customAttributes.Single().Value : null;
  }

  private static CustomAttribute? GetCustomAttribute(this OneTimePasswordModel oneTimePassword, string key)
  {
    CustomAttribute[] customAttributes = oneTimePassword.CustomAttributes.Where(x => x.Key == key).ToArray();
    if (customAttributes.Length > 1)
    {
      throw new ArgumentException($"Multiple ({customAttributes.Length}) custom attributes '{key}' were found for One-Time Password (Id={oneTimePassword.Id}).", nameof(oneTimePassword));
    }
    return customAttributes.SingleOrDefault();
  }
}
