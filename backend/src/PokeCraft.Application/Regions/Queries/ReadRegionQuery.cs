using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Permissions;
using PokeCraft.Application.Regions.Models;
using PokeCraft.Domain;

namespace PokeCraft.Application.Regions.Queries;

public record ReadRegionQuery(Guid? Id, string? UniqueName) : IRequest<RegionModel?>;

/// <exception cref="PermissionDeniedException"></exception>
/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadRegionQueryHandler : IRequestHandler<ReadRegionQuery, RegionModel?>
{
  private readonly IPermissionService _permissionService;
  private readonly IRegionQuerier _regionQuerier;

  public ReadRegionQueryHandler(IPermissionService permissionService, IRegionQuerier regionQuerier)
  {
    _permissionService = permissionService;
    _regionQuerier = regionQuerier;
  }

  public async Task<RegionModel?> Handle(ReadRegionQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanViewAsync(ResourceType.Region, cancellationToken);

    Dictionary<Guid, RegionModel> regions = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RegionModel? region = await _regionQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (region is not null)
      {
        regions[region.Id] = region;
      }
    }
    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      RegionModel? region = await _regionQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (region is not null)
      {
        regions[region.Id] = region;
      }
    }

    if (regions.Count > 1)
    {
      throw TooManyResultsException<RegionModel>.ExpectedSingle(regions.Count);
    }

    return regions.Values.SingleOrDefault();
  }
}
