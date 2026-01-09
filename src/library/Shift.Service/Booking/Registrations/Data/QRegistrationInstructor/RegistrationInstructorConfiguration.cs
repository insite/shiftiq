using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Booking;

public class RegistrationInstructorConfiguration : IEntityTypeConfiguration<RegistrationInstructorEntity>
{
    public void Configure(EntityTypeBuilder<RegistrationInstructorEntity> builder) 
    {
        builder.ToTable("QRegistrationInstructor", "registrations");
        builder.HasKey(x => new { x.InstructorIdentifier, x.RegistrationIdentifier });
            
        builder.Property(x => x.RegistrationIdentifier).HasColumnName("RegistrationIdentifier").IsRequired();
        builder.Property(x => x.InstructorIdentifier).HasColumnName("InstructorIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();

    }
}