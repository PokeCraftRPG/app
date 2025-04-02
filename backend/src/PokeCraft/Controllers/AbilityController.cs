using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Application.Abilities.Commands;
using PokeCraft.Application.Abilities.Models;
using PokeCraft.Application.Abilities.Queries;
using PokeCraft.Filters;

namespace PokeCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("abilities")]
public class AbilityController : ControllerBase
{
  private readonly IMediator _mediator;

  public AbilityController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<AbilityModel>> CreateAsync([FromBody] CreateOrReplaceAbilityPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceAbilityCommand command = new(Id: null, payload);
    CreateOrReplaceAbilityResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<AbilityModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadAbilityQuery query = new(id, UniqueName: null);
    AbilityModel? ability = await _mediator.Send(query, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<AbilityModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadAbilityQuery query = new(Id: null, uniqueName);
    AbilityModel? ability = await _mediator.Send(query, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<AbilityModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceAbilityPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceAbilityCommand command = new(id, payload);
    CreateOrReplaceAbilityResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<AbilityModel>> UpdateAsync(Guid id, [FromBody] UpdateAbilityPayload payload, CancellationToken cancellationToken)
  {
    UpdateAbilityCommand command = new(id, payload);
    AbilityModel? ability = await _mediator.Send(command, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  private ActionResult<AbilityModel> ToActionResult(CreateOrReplaceAbilityResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/abilities/{result.Ability.Id}", UriKind.Absolute);
      return Created(location, result.Ability);
    }

    return Ok(result.Ability);
  }
}
