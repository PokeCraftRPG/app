using Logitar.Portal.Contracts;

namespace PokeCraft.Application.Abilities.Models;

public record UpdateAbilityPayload
{
  public string? UniqueName { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public ChangeModel<string>? Link { get; set; }
  public ChangeModel<string>? Notes { get; set; }
}
