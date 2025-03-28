namespace PokeCraft.Application.Speciez.Models;

public record RegionalNumberPayload
{
  public string Region { get; set; }
  public int Number { get; set; }

  public RegionalNumberPayload() : this(string.Empty, number: 0)
  {
  }

  public RegionalNumberPayload(string region, int number)
  {
    Region = region;
    Number = number;
  }
}
