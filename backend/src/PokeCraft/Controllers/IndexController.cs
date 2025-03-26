using Microsoft.AspNetCore.Mvc;
using PokeCraft.Models.Index;

namespace PokeCraft.Controllers;

[ApiController]
[Route("")]
public class IndexController : ControllerBase
{
  [HttpGet]
  public ActionResult<ApiVersion> Get() => Ok(ApiVersion.Current);
}
