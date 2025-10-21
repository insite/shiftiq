using System.Data.Entity.ModelConfiguration;

using InSite.Application.Attempts.Read;

namespace InSite.Persistence
{
    public class QAttemptConfiguration : EntityTypeConfiguration<QAttempt>
    {
        public QAttemptConfiguration() : this("assessments") { }

        public QAttemptConfiguration(string schema)
        {
            ToTable(schema + ".QAttempt");
            HasKey(x => new { x.AttemptIdentifier });

            HasRequired(a => a.AssessorPerson).WithMany(b => b.AssessorAttempts).HasForeignKey(c => new { c.OrganizationIdentifier, c.AssessorUserIdentifier }).WillCascadeOnDelete(false);
            HasRequired(a => a.AssessorUser).WithMany(b => b.AssessorAttempts).HasForeignKey(c => c.AssessorUserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.LearnerPerson).WithMany(b => b.LearnerAttempts).HasForeignKey(c => new { c.OrganizationIdentifier, c.LearnerUserIdentifier }).WillCascadeOnDelete(false);
            HasRequired(a => a.LearnerUser).WithMany(b => b.LearnerAttempts).HasForeignKey(c => c.LearnerUserIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Form).WithMany(b => b.Attempts).HasForeignKey(c => c.FormIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Registration).WithMany(b => b.Attempts).HasForeignKey(c => c.RegistrationIdentifier).WillCascadeOnDelete(false);

            Property(x => x.AttemptGrade).IsOptional().IsUnicode(false).HasMaxLength(4);
            Property(x => x.AttemptPoints).HasPrecision(7, 2);
            Property(x => x.AttemptScore).HasPrecision(9, 8);
            Property(x => x.AttemptStatus).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AttemptTag).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.FormPoints).HasPrecision(7, 2);
            Property(x => x.UserAgent).IsUnicode(false).HasMaxLength(512);
            Property(x => x.TabTimeLimit).IsUnicode(false).HasMaxLength(8);
            Property(x => x.AttemptDuration).HasPrecision(12, 3);
            Property(x => x.AttemptLanguage).IsOptional().IsUnicode(false).HasMaxLength(2);
        }
    }
}
