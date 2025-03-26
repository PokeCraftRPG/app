using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application;

public interface IApplicationContext
{
  UserId UserId { get; }
  WorldId WorldId { get; }
}
