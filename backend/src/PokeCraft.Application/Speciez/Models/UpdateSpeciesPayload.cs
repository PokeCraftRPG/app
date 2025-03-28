using Logitar.Portal.Contracts;
using PokeCraft.Domain;

namespace PokeCraft.Application.Speciez.Models;

public record UpdateSpeciesPayload
{
  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }

  public int? BaseFriendship { get; set; }
  public int? CatchRate { get; set; }
  public GrowthRate? GrowthRate { get; set; }

  public List<RegionalNumberUpdatePayload> RegionalNumbers { get; set; } = [];

  public ChangeModel<string>? Link { get; set; }
  public ChangeModel<string>? Notes { get; set; }
}
