using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class QOrganizationConfiguration : IEntityTypeConfiguration<OrganizationEntity>
{
    public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
    {
        builder.ToTable("QOrganization", "accounts");
        builder.HasKey(x => new { x.OrganizationIdentifier });

        builder.Property(x => x.AccountClosed).HasColumnName("AccountClosed");
        builder.Property(x => x.AccountOpened).HasColumnName("AccountOpened");
        builder.Property(x => x.AccountStatus).HasColumnName("AccountStatus").IsRequired().IsUnicode(false).HasMaxLength(6);
        builder.Property(x => x.CompanyDomain).HasColumnName("CompanyDomain").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CompanyName).HasColumnName("CompanyName").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.CompanySize).HasColumnName("CompanySize").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.CompanySummary).HasColumnName("CompanySummary").IsUnicode(false).HasMaxLength(900);
        builder.Property(x => x.CompanyTitle).HasColumnName("CompanyTitle").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.CompanyWebSiteUrl).HasColumnName("CompanyWebSiteUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.CompetencyAutoExpirationMode).HasColumnName("CompetencyAutoExpirationMode").IsRequired().IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.CompetencyAutoExpirationMonth).HasColumnName("CompetencyAutoExpirationMonth");
        builder.Property(x => x.CompetencyAutoExpirationDay).HasColumnName("CompetencyAutoExpirationDay");
        builder.Property(x => x.StandardContentLabels).HasColumnName("StandardContentLabels").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.OrganizationCode).HasColumnName("OrganizationCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.OrganizationLogoUrl).HasColumnName("OrganizationLogoUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.TimeZone).HasColumnName("TimeZone").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.AdministratorUserIdentifier).HasColumnName("AdministratorUserIdentifier");
        builder.Property(x => x.GlossaryIdentifier).HasColumnName("GlossaryIdentifier");
        builder.Property(x => x.ParentOrganizationIdentifier).HasColumnName("ParentOrganizationIdentifier");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.OrganizationData).HasColumnName("OrganizationData").IsRequired().IsUnicode(false).HasMaxLength(7700);
        builder.Property(x => x.PersonFullNamePolicy).HasColumnName("PersonFullNamePolicy").IsUnicode(false).HasMaxLength(50);
    }
}