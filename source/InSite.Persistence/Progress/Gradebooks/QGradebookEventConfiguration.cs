using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    class QGradebookEventConfiguration : EntityTypeConfiguration<QGradebookEvent>
    {
        public QGradebookEventConfiguration() : this("records") { }

        public QGradebookEventConfiguration(string schema)
        {
            ToTable(schema + ".QGradebookEvent");
            HasKey(x => new { x.GradebookIdentifier, x.EventIdentifier });

            HasRequired(a => a.Gradebook).WithMany(b => b.GradebookEvents).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Event).WithMany(b => b.GradebookEvents).HasForeignKey(c => c.EventIdentifier).WillCascadeOnDelete(false);
        }
    }
}
