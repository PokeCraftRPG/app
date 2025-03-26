using Logitar.Portal.Contracts;
using MediatR;
using PokeCraft.Application.Regions.Models;

namespace PokeCraft.Application.Regions.Queries;

public record ReadRegionQuery(Guid? Id, string? UniqueName) : IRequest<RegionModel?>;

/// <exception cref="TooManyResultsException{T}"></exception>
internal class ReadRegionQueryHandler : IRequestHandler<ReadRegionQuery, RegionModel?>
{
  private readonly IRegionQuerier _regionQuerier;

  public ReadRegionQueryHandler(IRegionQuerier regionQuerier)
  {
    _regionQuerier = regionQuerier;
  }

  public async Task<RegionModel?> Handle(ReadRegionQuery query, CancellationToken cancellationToken)
  {
    // TODO(fpion): read permission

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
