using System.Data.Entity.ModelConfiguration;

using InSite.Application.Events.Read;

namespace InSite.Persistence
{
    public class QEventAssessmentFormConfiguration : EntityTypeConfiguration<QEventAssessmentForm>
    {
        public QEventAssessmentFormConfiguration() : this("events") { }

        public QEventAssessmentFormConfiguration(string schema)
        {
            ToTable(schema + ".QEventAssessmentForm");
            HasKey(x => new { x.EventIdentifier, x.FormIdentifier });

            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.BankIdentifier).IsRequired();
            Property(x => x.FormIdentifier).IsRequired();

            HasRequired(a => a.Form).WithMany(b => b.EventAssessmentForms).HasForeignKey(c => c.FormIdentifier).WillCascadeOnDelete(false);
        }
    }
}
