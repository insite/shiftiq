using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VProgramEnrollmentConfiguration : EntityTypeConfiguration<VProgramEnrollment>
    {
        public VProgramEnrollmentConfiguration() : this("records") { }

        public VProgramEnrollmentConfiguration(string schema)
        {
            ToTable(schema + ".VProgramEnrollment");
            HasKey(x => x.EnrollmentIdentifier);

            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ProgramCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ProgramDescription).IsOptional().IsUnicode(false);
            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.ProgramName).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserEmailAlternate).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserFullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserPhone).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ProgressCompleted).IsOptional();
            Property(x => x.ProgressStarted).IsOptional();
            Property(x => x.TimeTaken).IsOptional();
        }
    }
}
