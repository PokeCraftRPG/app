using Logitar.Data;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.PokemonDb;

internal static class Worlds
{
  public static readonly TableId Table = new(Schemas.Pokemon, nameof(PokemonContext.Worlds), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(WorldEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(WorldEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(WorldEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(WorldEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(WorldEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(WorldEntity.Version), Table);

  public static readonly ColumnId Description = new(nameof(WorldEntity.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(WorldEntity.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(WorldEntity.Id), Table);
  public static readonly ColumnId OwnerId = new(nameof(WorldEntity.OwnerId), Table);
  public static readonly ColumnId UniqueSlug = new(nameof(WorldEntity.UniqueSlug), Table);
  public static readonly ColumnId UniqueSlugNormalized = new(nameof(WorldEntity.UniqueSlugNormalized), Table);
  public static readonly ColumnId WorldId = new(nameof(WorldEntity.WorldId), Table);
}
