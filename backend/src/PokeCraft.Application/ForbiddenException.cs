namespace PokeCraft.Application;

public abstract class ForbiddenException : ErrorException
{
  protected ForbiddenException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
