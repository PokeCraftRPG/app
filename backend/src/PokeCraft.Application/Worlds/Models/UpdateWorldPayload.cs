using Logitar.Portal.Contracts;

namespace PokeCraft.Application.Worlds.Models;

public record UpdateWorldPayload
{
  public string? UniqueSlug { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }
}
