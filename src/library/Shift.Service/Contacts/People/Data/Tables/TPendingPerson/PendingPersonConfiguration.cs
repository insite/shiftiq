using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shift.Contract;

namespace Shift.Service.Directory;

public class PendingPersonConfiguration : IEntityTypeConfiguration<PendingPersonEntity>
{
    public void Configure(EntityTypeBuilder<PendingPersonEntity> builder)
    {
        builder.ToTable("TPendingPerson", "directory");
        builder.HasKey(x => new { x.PendingId });

        builder.Property(x => x.OrganizationId).HasColumnName("OrganizationId").IsRequired();
        builder.Property(x => x.SubmittedAt).HasColumnName("SubmittedAt").IsRequired();
        builder.Property(x => x.SubmittedBy).HasColumnName("SubmittedBy").IsRequired();
        builder.Property(x => x.PendingId).HasColumnName("PendingId").IsRequired();
        builder.Property(x => x.UserFirstName).HasColumnName("UserFirstName").IsRequired().IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.UserLastName).HasColumnName("UserLastName").IsRequired().IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.UserEmail).HasColumnName("UserEmail").IsRequired().IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.UserId).HasColumnName("UserId");
        builder.Property(x => x.PersonCode).HasColumnName("PersonCode").IsRequired().IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.PersonId).HasColumnName("PersonId");

    }
}

public class PendingPersonMatchConfiguration : IEntityTypeConfiguration<PendingPersonMatch>
{
    public void Configure(EntityTypeBuilder<PendingPersonMatch> builder)
    {
        builder.ToTable("VPendingPersonMatch", "directory");
        builder.HasKey(x => new { x.PendingId });

        builder.Ignore(e => e.SubmittedWhen);
    }
}
