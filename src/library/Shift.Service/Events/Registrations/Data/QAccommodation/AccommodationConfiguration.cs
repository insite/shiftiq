using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Booking;

public class AccommodationConfiguration : IEntityTypeConfiguration<AccommodationEntity>
{
    public void Configure(EntityTypeBuilder<AccommodationEntity> builder)
    {
        builder.ToTable("QAccommodation", "registrations");
        builder.HasKey(x => new { x.AccommodationIdentifier });

        builder.Property(x => x.AccommodationIdentifier).HasColumnName("AccommodationIdentifier").IsRequired();
        builder.Property(x => x.RegistrationIdentifier).HasColumnName("RegistrationIdentifier").IsRequired();
        builder.Property(x => x.AccommodationType).HasColumnName("AccommodationType").IsRequired().IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.AccommodationName).HasColumnName("AccommodationName").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.TimeExtension).HasColumnName("TimeExtension");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

        builder.HasOne(x => x.Registration)
            .WithMany(x => x.Accommodations)
            .HasForeignKey(x => x.RegistrationIdentifier)
            .HasPrincipalKey(x => x.RegistrationIdentifier);
    }
}
