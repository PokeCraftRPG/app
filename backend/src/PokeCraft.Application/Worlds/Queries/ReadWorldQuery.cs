using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Worlds.Models;

namespace PokeCraft.Application.Worlds.Queries;

public record ReadWorldQuery(Guid? Id, string? UniqueSlug) : IRequest<WorldModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadWorldQueryHandler : IRequestHandler<ReadWorldQuery, WorldModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IWorldQuerier _worldQuerier;

  public ReadWorldQueryHandler(IPermissionService permissionService, IWorldQuerier worldQuerier)
  {
    _permissionService = permissionService;
    _worldQuerier = worldQuerier;
  }

  public async Task<WorldModel?> Handle(ReadWorldQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, WorldModel> worlds = new(capacity: 2);

    if (query.Id.HasValue)
    {
      WorldModel? world = await _worldQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (world is not null)
      {
        await _permissionService.EnsureCanViewAsync(world, cancellationToken);

        worlds[world.Id] = world;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueSlug))
    {
      WorldModel? world = await _worldQuerier.ReadAsync(query.UniqueSlug, cancellationToken);
      if (world is not null)
      {
        await _permissionService.EnsureCanViewAsync(world, cancellationToken);

        worlds[world.Id] = world;
      }
    }

    if (worlds.Count > 1)
    {
      throw TooManyResultsException<WorldModel>.ExpectedSingle(worlds.Count);
    }

    return worlds.Values.SingleOrDefault();
  }
}
