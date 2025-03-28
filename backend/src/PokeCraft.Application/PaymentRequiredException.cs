namespace PokeCraft.Application;

public abstract class PaymentRequiredException : ErrorException
{
  protected PaymentRequiredException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
