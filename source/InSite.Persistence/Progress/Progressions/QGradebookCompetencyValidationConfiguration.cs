using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QGradebookCompetencyValidationConfiguration : EntityTypeConfiguration<QGradebookCompetencyValidation>
    {
        public QGradebookCompetencyValidationConfiguration() : this("records") { }

        public QGradebookCompetencyValidationConfiguration(string schema)
        {
            ToTable("QGradebookCompetencyValidation", schema);
            HasKey(x => x.ValidationIdentifier);

            Property(x => x.ValidationIdentifier).IsRequired();
            Property(x => x.CompetencyIdentifier).IsRequired();
            Property(x => x.GradebookIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.ValidationPoints).IsOptional();

            HasRequired(a => a.Gradebook).WithMany(b => b.Validations).HasForeignKey(c => c.GradebookIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Student).WithMany(b => b.Validations).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Standard).WithMany(b => b.Validations).HasForeignKey(c => c.CompetencyIdentifier).WillCascadeOnDelete(false);
        }
    }
}
