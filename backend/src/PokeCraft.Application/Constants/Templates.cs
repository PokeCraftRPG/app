﻿using PokeCraft.Application.Accounts;

namespace PokeCraft.Application.Constants;

internal static class Templates
{
  public const string AccountAuthentication = "AccountAuthentication";

  private const string MultiFactorAuthentication = "MultiFactorAuthentication{ContactType}";

  public static string GetMultiFactorAuthentication(ContactType contactType) => contactType switch
  {
    ContactType.Email or ContactType.Phone => MultiFactorAuthentication.Replace("{ContactType}", contactType.ToString()),
    _ => throw new ArgumentException($"The contact type '{contactType}' is not supported.", nameof(contactType)),
  };
}
