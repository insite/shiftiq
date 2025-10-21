using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class TUserStatusConfiguration : EntityTypeConfiguration<TUserStatus>
    {
        public TUserStatusConfiguration() : this("custom_cmds") { }

        public TUserStatusConfiguration(string schema)
        {
            ToTable(schema + ".TUserStatus");
            HasKey(x => x.JoinIdentifier);

            Property(x => x.JoinIdentifier).IsRequired();
            Property(x => x.AsAt).IsRequired();
            Property(x => x.CountCP).IsRequired();
            Property(x => x.CountEX).IsRequired();
            Property(x => x.CountNA).IsRequired();
            Property(x => x.CountNC).IsRequired();
            Property(x => x.CountNT).IsRequired();
            Property(x => x.CountRQ).IsRequired();
            Property(x => x.CountSA).IsRequired();
            Property(x => x.CountSV).IsRequired();
            Property(x => x.CountVA).IsRequired();
            Property(x => x.CountVN).IsRequired();
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.DepartmentName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DepartmentRole).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ItemName).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.ItemNumber).IsRequired();
            Property(x => x.ListDomain).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.ListFolder).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.Progress).IsOptional();
            Property(x => x.Score).IsOptional();
            Property(x => x.TagCriticality).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.TagNecessity).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.TagPrimacy).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.OrganizationName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserName).IsOptional().IsUnicode(false).HasMaxLength(100);
        }
    }
}
