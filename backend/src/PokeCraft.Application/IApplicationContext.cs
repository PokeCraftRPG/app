using PokeCraft.Domain;

namespace PokeCraft.Application;

public interface IApplicationContext
{
  UserId UserId { get; }
}
