﻿namespace PokeCraft.Application.Accounts.Models;

public record SignInAccountPayload
{
  public string Locale { get; set; }
  public Credentials? Credentials { get; set; }
  public string? AuthenticationToken { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }
  public CompleteProfilePayload? Profile { get; set; }

  public SignInAccountPayload() : this(string.Empty)
  {
  }

  public SignInAccountPayload(string locale)
  {
    Locale = locale;
  }
}
