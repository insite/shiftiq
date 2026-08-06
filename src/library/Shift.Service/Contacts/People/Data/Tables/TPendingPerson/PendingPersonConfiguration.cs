using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shift.Contract;

namespace Shift.Service.Directory;

public class PendingPersonConfiguration : IEntityTypeConfiguration<PendingPersonEntity>
{
    public void Configure(EntityTypeBuilder<PendingPersonEntity> builder)
    {
        builder.ToTable("TPendingPerson", "directory");
        builder.HasKey(x => new { x.PendingPersonIdentifier });

        builder.Property(x => x.PendingPersonIdentifier).IsRequired();
        builder.Property(x => x.OrganizationIdentifier).IsRequired();
        builder.Property(x => x.PersonCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.UserEmail).IsRequired().HasMaxLength(254);
        builder.Property(x => x.UserFirstName).IsRequired().HasMaxLength(40);
        builder.Property(x => x.UserLastName).IsRequired().HasMaxLength(40);
        builder.Property(x => x.UserMiddleName).HasMaxLength(38);
        builder.Property(x => x.JobDivision).HasMaxLength(100);
        builder.Property(x => x.JobTitle).HasMaxLength(256);
        builder.Property(x => x.WorkAddressStreet1).HasMaxLength(200);
        builder.Property(x => x.WorkAddressStreet2).HasMaxLength(200);
        builder.Property(x => x.WorkAddressCity).HasMaxLength(128);
        builder.Property(x => x.WorkAddressProvince).HasMaxLength(64);
        builder.Property(x => x.WorkAddressPostalCode).HasMaxLength(20);
        builder.Property(x => x.EmployeeStatus).IsRequired().HasMaxLength(200);
    }
}
