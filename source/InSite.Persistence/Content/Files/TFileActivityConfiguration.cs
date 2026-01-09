using System.Data.Entity.ModelConfiguration;

using InSite.Application.Files.Read;

namespace InSite.Persistence
{
    public class TFileActivityConfiguration : EntityTypeConfiguration<TFileActivity>
    {
        public TFileActivityConfiguration() : this("assets") { }

        public TFileActivityConfiguration(string schema)
        {
            ToTable(schema + ".TFileActivity");
            HasKey(x => new { x.ActivityIdentifier });

            Property(x => x.ActivityChanges).IsRequired().IsUnicode(false).HasMaxLength(1200);

            HasRequired(a => a.File).WithMany(b => b.FileActivities).HasForeignKey(c => c.FileIdentifier).WillCascadeOnDelete(false);
        }
    }
}
