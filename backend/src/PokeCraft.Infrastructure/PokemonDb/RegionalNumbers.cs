﻿using Logitar.Data;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.PokemonDb;

internal static class RegionalNumbers
{
  public static readonly TableId Table = new(Schemas.Pokemon, nameof(PokemonContext.RegionalNumbers), alias: null);

  public static readonly ColumnId Number = new(nameof(RegionalNumberEntity.Number), Table);
  public static readonly ColumnId RegionId = new(nameof(RegionalNumberEntity.RegionId), Table);
  public static readonly ColumnId RegionUid = new(nameof(RegionalNumberEntity.RegionUid), Table);
  public static readonly ColumnId SpeciesId = new(nameof(RegionalNumberEntity.SpeciesId), Table);
  public static readonly ColumnId SpeciesUid = new(nameof(RegionalNumberEntity.SpeciesUid), Table);
}
