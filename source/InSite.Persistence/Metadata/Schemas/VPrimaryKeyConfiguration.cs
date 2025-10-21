using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VPrimaryKeyConfiguration : EntityTypeConfiguration<VPrimaryKey>
    {
        public VPrimaryKeyConfiguration() : this("databases") { }

        public VPrimaryKeyConfiguration(string schema)
        {
            ToTable(schema + ".VPrimaryKey");
            HasKey(x => x.ConstraintName);

            Property(x => x.ColumnName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ConstraintName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.DataType).IsOptional().IsUnicode(false).HasMaxLength(42);
            Property(x => x.IsIdentity).IsRequired();
            Property(x => x.SchemaName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.TableName).IsRequired().IsUnicode(true).HasMaxLength(128);
        }
    }
}
