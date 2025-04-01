using PokeCraft.Domain.Speciez.Events;

namespace PokeCraft.Infrastructure.Entities;

internal class RegionalNumberEntity
{
  public SpeciesEntity? Species { get; set; }
  public int SpeciesId { get; private set; }
  public Guid SpeciesUid { get; private set; }

  public RegionEntity? Region { get; private set; }
  public int RegionId { get; private set; }
  public Guid RegionUid { get; private set; }

  public int Number { get; private set; }

  public RegionalNumberEntity(SpeciesEntity species, RegionEntity region, SpeciesRegionalNumberChanged @event)
  {
    Species = species;
    SpeciesId = species.SpeciesId;
    SpeciesUid = species.Id;

    Region = region;
    RegionId = region.RegionId;
    RegionUid = region.Id;

    Update(@event);
  }

  private RegionalNumberEntity()
  {
  }

  public void Update(SpeciesRegionalNumberChanged @event)
  {
    if (@event.Number is null)
    {
      throw new ArgumentException($"The {nameof(@event.Number)} is required.", nameof(@event));
    }

    Number = @event.Number.Value;
  }
}
