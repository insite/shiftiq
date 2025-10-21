using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VTableConfiguration : EntityTypeConfiguration<VTable>
    {
        public VTableConfiguration() : this("databases") { }

        public VTableConfiguration(string schema)
        {
            ToTable(schema + ".VTable");
            HasKey(x => x.TableId);
        
            Property(x => x.ColumnCount).IsRequired();
            Property(x => x.DateCreated).IsRequired();
            Property(x => x.DateModified).IsRequired();
            Property(x => x.HasClusteredIndex).IsOptional();
            Property(x => x.RowCount).IsOptional();
            Property(x => x.SchemaId).IsRequired();
            Property(x => x.SchemaName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.TableId).IsRequired();
            Property(x => x.TableName).IsRequired().IsUnicode(true).HasMaxLength(128);
        }
    }
}
