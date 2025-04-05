using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using PokeCraft.Application.Constants;

namespace PokeCraft.Application.Accounts;

public static class ApiKeyExtensions
{
  public static Guid GetUserId(this ApiKeyModel apiKey)
  {
    CustomAttribute customAttribute = apiKey.GetCustomAttribute(CustomAttributes.UserId)
      ?? throw new ArgumentException($"No custom attribute '{CustomAttributes.UserId}' was found for API key 'Id={apiKey.Id}'.", nameof(apiKey));
    return Guid.Parse(customAttribute.Value);
  }

  private static CustomAttribute? GetCustomAttribute(this ApiKeyModel apiKey, string key)
  {
    CustomAttribute[] customAttributes = apiKey.CustomAttributes.Where(x => x.Key == key).ToArray();
    if (customAttributes.Length > 1)
    {
      throw new ArgumentException($"Multiple ({customAttributes.Length}) custom attributes '{key}' were found for API key (Id={apiKey.Id}).", nameof(apiKey));
    }
    return customAttributes.SingleOrDefault();
  }
}
