using PokeCraft.Domain;

namespace PokeCraft.Infrastructure.PokemonDb;

internal static class Helper
{
  public static string Normalize(Slug slug) => Normalize(slug.Value);
  public static string Normalize(UniqueName uniqueName) => Normalize(uniqueName.Value);
  public static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
