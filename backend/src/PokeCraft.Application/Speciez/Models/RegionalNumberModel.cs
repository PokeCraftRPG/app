using PokeCraft.Application.Regions.Models;

namespace PokeCraft.Application.Speciez.Models;

public record RegionalNumberModel
{
  public RegionModel Region { get; set; }
  public int Number { get; set; }

  public RegionalNumberModel() : this(new RegionModel(), number: 0)
  {
  }

  public RegionalNumberModel(RegionModel region, int number)
  {
    Region = region;
    Number = number;
  }
}
