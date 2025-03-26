namespace PokeCraft.Application.Worlds.Models;

public record CreateOrReplaceWorldPayload
{
  public string UniqueSlug { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
}
