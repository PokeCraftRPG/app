using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;

namespace PokeCraft.Application.Speciez.Models;

public record CreateOrReplaceSpeciesPayload
{
  public int Number { get; set; }
  public SpeciesCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public int BaseFriendship { get; set; }
  public int CatchRate { get; set; }
  public GrowthRate GrowthRate { get; set; }

  public List<RegionalNumberPayload> RegionalNumbers { get; set; } = [];

  public string? Link { get; set; }
  public string? Notes { get; set; }
}
