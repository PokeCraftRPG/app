using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application;

public interface IApplicationContext
{
  UserId UserId { get; }
  WorldModel World { get; }
  WorldId WorldId { get; }
}
