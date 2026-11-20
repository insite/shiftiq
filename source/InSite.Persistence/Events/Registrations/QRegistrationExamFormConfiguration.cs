using System.Data.Entity.ModelConfiguration;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence
{
    public class QRegistrationExamFormConfiguration : EntityTypeConfiguration<QRegistrationExamForm>
    {
        public QRegistrationExamFormConfiguration() : this("registrations") { }

        public QRegistrationExamFormConfiguration(string schema)
        {
            ToTable(schema + ".QRegistrationExamForm");
            HasKey(x => x.RegistrationExamFormIdentifier);

            HasRequired(a => a.Registration).WithMany(b => b.ExamForms).HasForeignKey(c => c.RegistrationIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Form).WithMany(b => b.RegistrationExamForms).HasForeignKey(c => c.ExamFormIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Attempt).WithMany(b => b.RegistrationExamForms).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);

            Property(x => x.Password).IsRequired().IsUnicode(false).HasMaxLength(14);
            Property(x => x.EligibilityStatus).IsUnicode(false).HasMaxLength(30);
        }
    }
}
