using Logitar.Portal.Contracts;
using PokeCraft.Application.Worlds.Models;

namespace PokeCraft.Application.Regions.Models;

public class RegionModel : AggregateModel
{
  public WorldModel World { get; set; } = new();

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Link { get; set; }
  public string? Notes { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
