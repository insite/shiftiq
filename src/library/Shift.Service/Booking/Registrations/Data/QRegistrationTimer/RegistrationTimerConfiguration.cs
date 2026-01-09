using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Booking;

public class RegistrationTimerConfiguration : IEntityTypeConfiguration<RegistrationTimerEntity>
{
    public void Configure(EntityTypeBuilder<RegistrationTimerEntity> builder) 
    {
        builder.ToTable("QRegistrationTimer", "registrations");
        builder.HasKey(x => new { x.TriggerCommand });
            
        builder.Property(x => x.RegistrationIdentifier).HasColumnName("RegistrationIdentifier").IsRequired();
        builder.Property(x => x.TimerDescription).HasColumnName("TimerDescription").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.TimerStatus).HasColumnName("TimerStatus").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.TriggerCommand).HasColumnName("TriggerCommand").IsRequired();
        builder.Property(x => x.TriggerTime).HasColumnName("TriggerTime").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}