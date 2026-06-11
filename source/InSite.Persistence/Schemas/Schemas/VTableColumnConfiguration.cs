using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VTableColumnConfiguration : EntityTypeConfiguration<VTableColumn>
    {
        public VTableColumnConfiguration() : this("databases") { }

        public VTableColumnConfiguration(string schema)
        {
            ToTable(schema + ".VTableColumn");
            HasKey(x => new { x.SchemaName, x.TableName, x.ColumnName });
        
            Property(x => x.ColumnName).IsRequired().HasMaxLength(128);
            Property(x => x.DataType).IsRequired().HasMaxLength(128);
            Property(x => x.SchemaName).IsRequired().HasMaxLength(128);
            Property(x => x.TableName).IsRequired().HasMaxLength(128);
        }
    }
}
