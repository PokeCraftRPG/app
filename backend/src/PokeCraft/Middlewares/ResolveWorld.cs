using MediatR;
using Microsoft.Extensions.Primitives;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Worlds;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Application.Worlds.Queries;
using PokeCraft.Constants;
using PokeCraft.Extensions;

namespace PokeCraft.Middlewares;

internal class ResolveWorld
{
  private readonly RequestDelegate _next;

  public ResolveWorld(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, IPermissionService permissionService, IMediator mediator)
  {
    HttpRequest request = context.Request;
    HttpResponse response = context.Response;

    if (request.Headers.TryGetValue(Headers.World, out StringValues values))
    {
      string? idOrUniqueSlug = values.Single();
      if (!string.IsNullOrWhiteSpace(idOrUniqueSlug))
      {
        ReadWorldQuery query = new(Guid.TryParse(idOrUniqueSlug, out Guid id) ? id : null, idOrUniqueSlug);
        WorldModel world = await mediator.Send(query) ?? throw new WorldNotFoundException(idOrUniqueSlug, Headers.World);
        context.SetWorld(world);
      }
    }

    await _next(context);
  }
}
