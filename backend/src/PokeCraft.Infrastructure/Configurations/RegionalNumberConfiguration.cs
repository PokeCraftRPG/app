using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Configurations;

internal class RegionalNumberConfiguration : IEntityTypeConfiguration<RegionalNumberEntity>
{
  public void Configure(EntityTypeBuilder<RegionalNumberEntity> builder)
  {
    builder.ToTable(RegionalNumbers.Table.Table ?? string.Empty, RegionalNumbers.Table.Schema);
    builder.HasKey(x => new { x.SpeciesId, x.RegionId });

    builder.HasIndex(x => new { x.RegionId, x.Number }).IsUnique();
    builder.HasIndex(x => new { x.RegionUid, x.Number }).IsUnique();

    builder.HasOne(x => x.Species).WithMany(x => x.RegionalNumbers)
      .HasPrincipalKey(x => x.SpeciesId).HasForeignKey(x => x.SpeciesId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Region).WithMany(x => x.RegionalNumbers)
      .HasPrincipalKey(x => x.RegionId).HasForeignKey(x => x.RegionId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
