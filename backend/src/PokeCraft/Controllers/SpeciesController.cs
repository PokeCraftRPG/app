using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Application.Speciez.Commands;
using PokeCraft.Application.Speciez.Models;
using PokeCraft.Application.Speciez.Queries;

namespace PokeCraft.Controllers;

[ApiController]
[Authorize] // TODO(fpion): RequireWorld
[Route("species")]
public class SpeciesController : ControllerBase
{
  private readonly IMediator _mediator;

  public SpeciesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<SpeciesModel>> CreateAsync([FromBody] CreateOrReplaceSpeciesPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpeciesCommand command = new(Id: null, payload);
    CreateOrReplaceSpeciesResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadSpeciesQuery query = new(id, Number: null, UniqueName: null, Region: null);
    SpeciesModel? species = await _mediator.Send(query, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadSpeciesQuery query = new(Id: null, Number: null, uniqueName, Region: null);
    SpeciesModel? species = await _mediator.Send(query, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("number:{number}")]
  public async Task<ActionResult<SpeciesModel>> ReadAsync(int number, string? region, CancellationToken cancellationToken)
  {
    ReadSpeciesQuery query = new(Id: null, number, UniqueName: null, region);
    SpeciesModel? species = await _mediator.Send(query, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<SpeciesModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceSpeciesPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceSpeciesCommand command = new(id, payload);
    CreateOrReplaceSpeciesResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<SpeciesModel>> UpdateAsync(Guid id, [FromBody] UpdateSpeciesPayload payload, CancellationToken cancellationToken)
  {
    UpdateSpeciesCommand command = new(id, payload);
    SpeciesModel? species = await _mediator.Send(command, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  private ActionResult<SpeciesModel> ToActionResult(CreateOrReplaceSpeciesResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/species/{result.Species.Id}", UriKind.Absolute);
      return Created(location, result.Species);
    }

    return Ok(result.Species);
  }
}
