using Logitar.Portal.Contracts.Actors;
using PokeCraft.Application;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft;

internal class TestApplicationContext : IApplicationContext
{
  public UserId UserId { get; }
  public WorldModel World { get; set; } = new(); // TODO(fpion): implement
  public WorldId WorldId => new(World.Id);

  public TestApplicationContext(ActorModel actor)
  {
    UserId = new UserId(actor.Id);
  }
}
