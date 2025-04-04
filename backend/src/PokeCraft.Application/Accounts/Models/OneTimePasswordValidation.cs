﻿using Logitar.Portal.Contracts.Passwords;

namespace PokeCraft.Application.Accounts.Models;

public record OneTimePasswordValidation
{
  public Guid Id { get; set; }
  public SentMessage SentMessage { get; set; }

  public OneTimePasswordValidation() : this(Guid.Empty, new())
  {
  }

  public OneTimePasswordValidation(OneTimePasswordModel oneTimePassword, SentMessage sentMessage) : this(oneTimePassword.Id, sentMessage)
  {
  }

  public OneTimePasswordValidation(Guid id, SentMessage sentMessage)
  {
    Id = id;
    SentMessage = sentMessage;
  }
}
