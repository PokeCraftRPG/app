using Logitar.Portal.Contracts.Users;
using PokeCraft.Application;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;
using PokeCraft.Extensions;

namespace PokeCraft;

internal class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public UserId UserId
  {
    get
    {
      UserModel user = Context.GetUser() ?? throw new InvalidOperationException("An authenticated user is required.");
      return new UserId(user.Id);
    }
  }

  public WorldModel World => Context.GetWorld() ?? throw new InvalidOperationException("A world is required.");
  public WorldId WorldId => new(World.Id);
}
