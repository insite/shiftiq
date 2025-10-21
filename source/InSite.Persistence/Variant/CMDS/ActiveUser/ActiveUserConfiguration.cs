using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class ActiveUserConfiguration : EntityTypeConfiguration<ActiveUser>
    {
        public ActiveUserConfiguration() : this("custom_cmds") { }

        public ActiveUserConfiguration(string schema)
        {
            ToTable(schema + ".ActiveUser");
            HasKey(x => new { x.Email });

            Property(x => x.Email).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.FirstName).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.FullName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.LastName).IsOptional().IsUnicode(false).HasMaxLength(80);
            Property(x => x.MiddleName).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UtcAuthenticated).IsOptional();
        }
    }
}
