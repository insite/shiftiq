using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class QGroupConfiguration : EntityTypeConfiguration<QGroup>
    {
        public QGroupConfiguration() : this("contacts") { }

        public QGroupConfiguration(string schema)
        {
            ToTable(schema + ".QGroup");
            HasKey(x => new { x.GroupIdentifier });

            Property(x => x.GroupCategory).IsUnicode(false).HasMaxLength(120);
            Property(x => x.GroupCode).IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupType).IsUnicode(false).IsRequired().HasMaxLength(32);
            Property(x => x.GroupDescription).IsUnicode(false).IsMaxLength();
            Property(x => x.GroupFax).IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.GroupImage).IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupIndustry).IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupIndustryComment).IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupLabel).IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupName).IsUnicode(false).IsRequired().HasMaxLength(90);
            Property(x => x.GroupOffice).IsUnicode(false).HasMaxLength(800);
            Property(x => x.GroupPhone).IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupRegion).IsUnicode(false).HasMaxLength(30);
            Property(x => x.GroupSize).IsUnicode(false).HasMaxLength(100);
            Property(x => x.GroupWebSiteUrl).IsUnicode(false).HasMaxLength(500);
            Property(x => x.ShippingPreference).IsUnicode(false).HasMaxLength(20);
            Property(x => x.SurveyNecessity).IsUnicode(false).HasMaxLength(50);
            Property(x => x.LastChangeType).IsUnicode(false).IsRequired().HasMaxLength(100);
            Property(x => x.LastChangeUser).IsUnicode(false).IsRequired().HasMaxLength(100);
            Property(x => x.SocialMediaUrls).IsUnicode(false);

            HasOptional(a => a.Parent).WithMany(b => b.Children).HasForeignKey(c => c.ParentGroupIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Organization).WithMany(b => b.Groups).HasForeignKey(c => c.OrganizationIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.MembershipProduct).WithMany(b => b.MembershipGroups).HasForeignKey(c => c.MembershipProductIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.SurveyForm).WithMany(b => b.QGroups).HasForeignKey(c => c.SurveyFormIdentifier).WillCascadeOnDelete(false);
        }
    }
}
