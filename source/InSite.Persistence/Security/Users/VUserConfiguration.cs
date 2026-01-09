using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class VUserConfiguration : EntityTypeConfiguration<VUser>
    {
        public VUserConfiguration() : this("identities") { }

        public VUserConfiguration(string schema)
        {
            ToTable(schema + ".VUser");
            HasKey(x => x.UserIdentifier);

            Property(x => x.UserEmail).IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFirstName).IsUnicode(false).HasMaxLength(64);
            Property(x => x.UserLastName).IsUnicode(false).HasMaxLength(80);
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.UserTimeZone).IsRequired().IsUnicode(false).HasMaxLength(32);
        }
    }
}