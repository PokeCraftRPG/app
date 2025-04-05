using PokeCraft.Domain;

namespace PokeCraft.Application.Accounts;

public record InvalidCredentialsError : Error
{
  public InvalidCredentialsError() : base("InvalidCredentials", "The specified credentials did not match.")
  {
  }
}
