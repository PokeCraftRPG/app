using Microsoft.AspNetCore.Mvc.Filters;
using PokeCraft.Extensions;

namespace PokeCraft.Filters;

internal class RequireWorldAttribute : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    if (context.HttpContext.GetWorld() is null)
    {
      throw new NotImplementedException(); // TODO(fpion): error handling
    }
    else
    {
      base.OnActionExecuting(context);
    }
  }
}
