﻿using PokeCraft.Domain;

namespace PokeCraft.Application;

public abstract class ConflictException : ErrorException
{
  protected ConflictException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
