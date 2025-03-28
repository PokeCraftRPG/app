using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;

namespace PokeCraft.Application.Worlds.Models;

public class WorldModel : AggregateModel
{
  public ActorModel Owner { get; set; } = new();

  public string UniqueSlug { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
