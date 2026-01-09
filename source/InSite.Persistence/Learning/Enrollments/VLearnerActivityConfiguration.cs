using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VLearnerActivityConfiguration : EntityTypeConfiguration<VLearnerActivity>
    {
        public VLearnerActivityConfiguration() : this("records") { }

        public VLearnerActivityConfiguration(string schema)
        {
            ToTable(schema + ".VLearnerActivity");
            HasKey(x => new { x.LearnerIdentifier, x.ProgramIdentifier });

            Property(x => x.AchievementIdentifier).IsOptional();
            Property(x => x.AchievementLabel).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.AchievementTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.CertificateGranted).IsOptional();
            Property(x => x.CertificateStatus).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.EnrollmentStarted).IsRequired();
            Property(x => x.EnrollmentStatus).IsRequired().IsUnicode(false).HasMaxLength(9);
            Property(x => x.GradebookIdentifier).IsOptional();
            Property(x => x.GradebookName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ImmigrationArrival).IsOptional();
            Property(x => x.ImmigrationDestination).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ImmigrationNumber).IsOptional().IsUnicode(false).HasMaxLength(64);
            Property(x => x.ImmigrationStatus).IsOptional().IsUnicode(false).HasMaxLength(4);
            Property(x => x.LearnerCitizenship).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LearnerCreated).IsRequired();
            Property(x => x.LearnerEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.LearnerGender).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.LearnerIdentifier).IsRequired();
            Property(x => x.LearnerNameFirst).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.LearnerNameLast).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.ProgramName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.ReferrerName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ReferrerOther).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.SessionMinutes).IsOptional();
            Property(x => x.SessionStartedFirst).IsOptional();
            Property(x => x.SessionStartedLast).IsOptional();
            Property(x => x.OrganizationIdentifier).IsOptional();
            Property(x => x.PersonCode).IsOptional();
        }
    }
}
