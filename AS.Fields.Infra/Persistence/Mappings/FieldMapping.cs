using AS.Fields.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.Fields.Infra.Persistence.Mappings
{
    public class FieldMapping : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.ToTable("fields");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Property)
                   .WithMany(p => p.Fields)
                   .HasForeignKey(x => x.PropertyId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired(false);

            builder.OwnsOne(x => x.Boundary, boundary =>
            {
                boundary.Property(b => b.LatMax)
                    .IsRequired();

                boundary.Property(b => b.LatMin)
                    .IsRequired();

                boundary.Property(b => b.LongMax)
                    .IsRequired();

                boundary.Property(b => b.LongMin)
                    .IsRequired();

                boundary.HasIndex(b => new { b.LatMin, b.LatMax })
                        .HasDatabaseName("IX_Fields_Boundary_Lat");

                boundary.HasIndex(b => new { b.LongMin, b.LongMax })
                        .HasDatabaseName("IX_Fields_Boundary_Long");
            });

            builder.Property(x => x.Description)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(x => x.Crop)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(x => x.PlantingDate);

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.Observations)
                   .HasMaxLength(255);
        }
    }
}
