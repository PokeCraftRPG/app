using Microsoft.AspNetCore.Mvc.Filters;
using PokeCraft.Constants;
using PokeCraft.Extensions;

namespace PokeCraft.Filters;

internal class RequireWorldAttribute : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if (context.HttpContext.GetWorld() is null)
    {
      throw new WorldIsRequiredException(Headers.World);
    }
    else
    {
      base.OnActionExecuting(context);
    }
  }
}
