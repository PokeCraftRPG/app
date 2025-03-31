using PokeCraft.Application;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft;

internal class HttpApplicationContext : IApplicationContext
{
  public UserId UserId => throw new NotImplementedException(); // TODO(fpion): authentication
  public WorldModel World => throw new NotImplementedException(); // TODO(fpion): resolve world
  public WorldId WorldId => new(World.Id);
}
