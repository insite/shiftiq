using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VForeignKeyConstraintConfiguration : EntityTypeConfiguration<VForeignKeyConstraint>
    {
        public VForeignKeyConstraintConfiguration() : this("databases") { }

        public VForeignKeyConstraintConfiguration(string schema)
        {
            ToTable(schema + ".VForeignKeyConstraint");
            HasKey(x => x.ConstraintName );
        
            Property(x => x.ConstraintName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ForeignColumnName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ForeignSchemaName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ForeignTableName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.PrimaryColumnName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.PrimarySchemaName).IsOptional().IsUnicode(true).HasMaxLength(128);
            Property(x => x.PrimaryTableName).IsOptional().IsUnicode(true).HasMaxLength(128);
        }
    }
}
