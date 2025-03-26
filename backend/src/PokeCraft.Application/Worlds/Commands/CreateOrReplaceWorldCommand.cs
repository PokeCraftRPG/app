﻿using FluentValidation;
using MediatR;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Application.Worlds.Validators;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds.Commands;

public record CreateOrReplaceWorldResult(WorldModel World, bool Created);

public record CreateOrReplaceWorldCommand(Guid? Id, CreateOrReplaceWorldPayload Payload) : IRequest<CreateOrReplaceWorldResult>;

/// <exception cref="UniqueSlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
internal class CreateOrReplaceWorldCommandHandler : IRequestHandler<CreateOrReplaceWorldCommand, CreateOrReplaceWorldResult>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IWorldManager _worldManager;
  private readonly IWorldQuerier _worldQuerier;
  private readonly IWorldRepository _worldRepository;

  public CreateOrReplaceWorldCommandHandler(
    IApplicationContext applicationContext,
    IWorldManager worldManager,
    IWorldQuerier worldQuerier,
    IWorldRepository worldRepository)
  {
    _applicationContext = applicationContext;
    _worldManager = worldManager;
    _worldQuerier = worldQuerier;
    _worldRepository = worldRepository;
  }

  public async Task<CreateOrReplaceWorldResult> Handle(CreateOrReplaceWorldCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceWorldPayload payload = command.Payload;
    new CreateOrReplaceWorldValidator().ValidateAndThrow(payload);

    WorldId id = WorldId.NewId();
    World? world = null;
    if (command.Id.HasValue)
    {
      id = new(command.Id.Value);
      world = await _worldRepository.LoadAsync(id, cancellationToken);
    }

    UserId ownerId = _applicationContext.UserId;
    Slug uniqueSlug = new(payload.UniqueSlug);

    bool created = false;
    if (world is null)
    {
      // TODO(fpion): create permission

      world = new(ownerId, uniqueSlug, id);
      created = true;
    }
    else
    {
      // TODO(fpion): update permission

      world.UniqueSlug = uniqueSlug;
    }

    world.DisplayName = DisplayName.TryCreate(payload.DisplayName);
    world.Description = Description.TryCreate(payload.Description);

    world.Update(ownerId);
    await _worldManager.SaveAsync(world, cancellationToken);

    WorldModel model = await _worldQuerier.ReadAsync(world, cancellationToken);
    return new CreateOrReplaceWorldResult(model, created);
  }
}
