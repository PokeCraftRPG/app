using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeCraft.Domain;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Configurations;

internal class RegionConfiguration : AggregateConfiguration<RegionEntity>
{
  public override void Configure(EntityTypeBuilder<RegionEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(Regions.Table.Table ?? string.Empty, Regions.Table.Schema);
    builder.HasKey(x => x.RegionId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldUid, x.Id }).IsUnique();
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.WorldId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => new { x.WorldUid, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Link).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Regions)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
