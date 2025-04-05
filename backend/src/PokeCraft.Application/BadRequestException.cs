using PokeCraft.Domain;

namespace PokeCraft.Application;

public abstract class BadRequestException : ErrorException
{
  protected BadRequestException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
