using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Application.Moves.Commands;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Application.Moves.Queries;
using PokeCraft.Filters;

namespace PokeCraft.Controllers;

[ApiController]
[Authorize]
[RequireWorld]
[Route("moves")]
public class MoveController : ControllerBase
{
  private readonly IMediator _mediator;

  public MoveController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<ActionResult<MoveModel>> CreateAsync([FromBody] CreateOrReplaceMovePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceMoveCommand command = new(Id: null, payload);
    CreateOrReplaceMoveResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<MoveModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadMoveQuery query = new(id, UniqueName: null);
    MoveModel? move = await _mediator.Send(query, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<MoveModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ReadMoveQuery query = new(Id: null, uniqueName);
    MoveModel? move = await _mediator.Send(query, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<MoveModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceMovePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceMoveCommand command = new(id, payload);
    CreateOrReplaceMoveResult result = await _mediator.Send(command, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<MoveModel>> UpdateAsync(Guid id, [FromBody] UpdateMovePayload payload, CancellationToken cancellationToken)
  {
    UpdateMoveCommand command = new(id, payload);
    MoveModel? move = await _mediator.Send(command, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  private ActionResult<MoveModel> ToActionResult(CreateOrReplaceMoveResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/moves/{result.Move.Id}", UriKind.Absolute);
      return Created(location, result.Move);
    }

    return Ok(result.Move);
  }
}
