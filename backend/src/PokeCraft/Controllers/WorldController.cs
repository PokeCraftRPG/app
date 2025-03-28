using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Application.Worlds.Commands;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Application.Worlds.Queries;

namespace PokeCraft.Controllers;

[ApiController]
[Authorize]
[Route("worlds")]
public class WorldController : ControllerBase
{
  private readonly IMediator _mediator;

  public WorldController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<WorldModel>> CreateAsync([FromBody] CreateOrReplaceWorldPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldCommand command = new(Id: null, payload);
    CreateOrReplaceWorldResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<WorldModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadWorldQuery query = new(id, UniqueSlug: null);
    WorldModel? world = await _mediator.Send(query, cancellationToken);
    return world is null ? NotFound() : Ok(world);
  }

  [HttpGet("slug:{uniqueSlug}")]
  public async Task<ActionResult<WorldModel>> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    ReadWorldQuery query = new(Id: null, uniqueSlug);
    WorldModel? world = await _mediator.Send(query, cancellationToken);
    return world is null ? NotFound() : Ok(world);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<WorldModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceWorldPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldCommand command = new(id, payload);
    CreateOrReplaceWorldResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<WorldModel>> UpdateAsync(Guid id, [FromBody] UpdateWorldPayload payload, CancellationToken cancellationToken)
  {
    UpdateWorldCommand command = new(id, payload);
    WorldModel? world = await _mediator.Send(command, cancellationToken);
    return world is null ? NotFound() : Ok(world);
  }

  private ActionResult<WorldModel> ToActionResult(CreateOrReplaceWorldResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/worlds/{result.World.Id}", UriKind.Absolute);
      return Created(location, result.World);
    }

    return Ok(result.World);
  }
}
