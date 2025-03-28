namespace PokeCraft.Application.Speciez.Models;

public record RegionalNumberUpdatePayload
{
  public string Region { get; set; }
  public int? Number { get; set; }

  public RegionalNumberUpdatePayload() : this(string.Empty)
  {
  }

  public RegionalNumberUpdatePayload(string region, int? number = null)
  {
    Region = region;
    Number = number;
  }
}
