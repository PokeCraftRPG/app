using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Application.Regions.Commands;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Application.Regions.Queries;
using PokeCraft.Filters;

namespace PokeCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("regions")]
public class RegionController : ControllerBase
{
  private readonly IMediator _mediator;

  public RegionController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<RegionModel>> CreateAsync([FromBody] CreateOrReplaceRegionPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegionCommand command = new(Id: null, payload);
    CreateOrReplaceRegionResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RegionModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRegionQuery query = new(id, UniqueName: null);
    RegionModel? region = await _mediator.Send(query, cancellationToken);
    return region is null ? NotFound() : Ok(region);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<RegionModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadRegionQuery query = new(Id: null, uniqueName);
    RegionModel? region = await _mediator.Send(query, cancellationToken);
    return region is null ? NotFound() : Ok(region);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RegionModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRegionPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRegionCommand command = new(id, payload);
    CreateOrReplaceRegionResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RegionModel>> UpdateAsync(Guid id, [FromBody] UpdateRegionPayload payload, CancellationToken cancellationToken)
  {
    UpdateRegionCommand command = new(id, payload);
    RegionModel? region = await _mediator.Send(command, cancellationToken);
    return region is null ? NotFound() : Ok(region);
  }

  private ActionResult<RegionModel> ToActionResult(CreateOrReplaceRegionResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/regions/{result.Region.Id}", UriKind.Absolute);
      return Created(location, result.Region);
    }

    return Ok(result.Region);
  }
}
