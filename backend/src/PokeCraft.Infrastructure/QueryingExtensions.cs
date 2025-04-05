using PokeCraft.Domain.Worlds;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure;

internal static class QueryingExtensions
{
  public static IQueryable<T> WhereWorld<T>(this IQueryable<T> query, WorldId worldId) where T : ISegregatedEntity
  {
    Guid worldUid = worldId.ToGuid();
    return query.Where(x => x.WorldUid == worldUid);
  }
}
