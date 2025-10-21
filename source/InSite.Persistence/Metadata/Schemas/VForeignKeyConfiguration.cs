using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VForeignKeyConfiguration : EntityTypeConfiguration<VForeignKey>
    {
        public VForeignKeyConfiguration() : this("databases") { }

        public VForeignKeyConfiguration(string schema)
        {
            ToTable(schema + ".VForeignKey");
            HasKey(x => x.UniqueName);
        
            Property(x => x.ForeignColumnName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ForeignColumnRequired).IsOptional();
            Property(x => x.ForeignSchemaId).IsRequired();
            Property(x => x.ForeignSchemaName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ForeignTableId).IsRequired();
            Property(x => x.ForeignTableName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.IsEnforced).IsOptional();
            Property(x => x.PrimaryColumnName).IsRequired().IsUnicode(true).HasMaxLength(138);
            Property(x => x.PrimarySchemaId).IsRequired();
            Property(x => x.PrimarySchemaName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.PrimaryTableId).IsRequired();
            Property(x => x.PrimaryTableName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.RowNumber).IsOptional();
            Property(x => x.UniqueName).IsRequired().IsUnicode(true).HasMaxLength(257);
        }
    }
}
