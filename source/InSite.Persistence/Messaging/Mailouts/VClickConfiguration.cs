using System.Data.Entity.ModelConfiguration;

using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class VClickConfiguration : EntityTypeConfiguration<VClick>
    {
        public VClickConfiguration() : this("messages") { }

        public VClickConfiguration(string schema)
        {
            ToTable(schema + ".VClick");
            HasKey(x => new { x.ClickIdentifier, x.MessageIdentifier, x.LinkIdentifier, x.UserIdentifier });

            Property(x => x.ClickIdentifier).IsRequired();
            Property(x => x.LinkIdentifier).IsRequired();
            Property(x => x.MessageIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.LinkText).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.LinkUrl).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.MessageTitle).IsUnicode(false).HasMaxLength(256);
            Property(x => x.UserBrowser).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.UserHostAddress).IsRequired().IsUnicode(false).HasMaxLength(15);
            Property(x => x.Clicked).IsRequired();
        }
    }
}
