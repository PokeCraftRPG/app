﻿using PokeCraft.Domain;

namespace PokeCraft.Application;

public abstract class NotFoundException : ErrorException
{
  protected NotFoundException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
