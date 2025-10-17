using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Booking;

public class EventUserConfiguration : IEntityTypeConfiguration<EventUserEntity>
{
    public void Configure(EntityTypeBuilder<EventUserEntity> builder) 
    {
        builder.ToTable("QEventAttendee", "events");
        builder.HasKey(x => new { x.EventIdentifier, x.UserIdentifier });
            
            builder.Property(x => x.EventIdentifier).HasColumnName("EventIdentifier").IsRequired();
            builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
            builder.Property(x => x.AttendeeRole).HasColumnName("AttendeeRole").IsRequired().IsUnicode(false).HasMaxLength(200);
            builder.Property(x => x.Assigned).HasColumnName("Assigned");
            builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();

    }
}