using PokeCraft.Application.Regions.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Regions;

namespace PokeCraft.Application.Regions;

public interface IRegionQuerier
{
  Task<RegionId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<IReadOnlyDictionary<Guid, string>> GetUniqueNameByIdsAsync(CancellationToken cancellationToken = default);

  Task<RegionModel> ReadAsync(Region region, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RegionModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);
}
