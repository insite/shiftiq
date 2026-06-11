using System.Data.Entity.ModelConfiguration;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    internal class QModuleConfiguration : EntityTypeConfiguration<QModule>
    {
        public QModuleConfiguration() : this("courses") { }

        public QModuleConfiguration(string schema)
        {
            ToTable(schema + ".QModule");
            HasKey(x => x.ModuleIdentifier);

            Property(x => x.ModuleCode).IsUnicode(false).HasMaxLength(30);
            Property(x => x.ModuleImage).IsUnicode(false).HasMaxLength(200);
            Property(x => x.ModuleName).IsUnicode(false).HasMaxLength(200);

            HasRequired(a => a.Unit).WithMany(b => b.Modules).HasForeignKey(a => a.UnitIdentifier).WillCascadeOnDelete(false);
        }
    }
}
