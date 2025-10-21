using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public class TModuleConfiguration : EntityTypeConfiguration<TModule>
    {
        public TModuleConfiguration() : this("courses") { }

        public TModuleConfiguration(string schema)
        {
            ToTable(schema + ".TModule");
            HasKey(x => new { x.ModuleIdentifier });

            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
            Property(x => x.GradeCategoryIdentifier).IsOptional();
            Property(x => x.Modified).IsRequired();
            Property(x => x.ModifiedBy).IsRequired();
            Property(x => x.ModuleAsset).IsRequired();
            Property(x => x.ModuleCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ModuleIdentifier).IsRequired();
            Property(x => x.ModuleImage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ModuleName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ModuleSequence).IsRequired();
            Property(x => x.SourceIdentifier).IsOptional();
            Property(x => x.UnitIdentifier).IsRequired();

            HasRequired(a => a.Unit).WithMany(b => b.Modules).HasForeignKey(a => a.UnitIdentifier).WillCascadeOnDelete(false);
        }
    }
}
