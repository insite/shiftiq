using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramGroupEnrollmentConfiguration : EntityTypeConfiguration<TProgramGroupEnrollment>
    {
        public TProgramGroupEnrollmentConfiguration() : this("records") { }

        public TProgramGroupEnrollmentConfiguration(string schema)
        {
            ToTable(schema + ".TProgramGroupEnrollment");
            HasKey(x => x.ProgramGroupEnrollmentIdentifier);

            HasRequired(x => x.Program).WithMany(x => x.GroupEnrollments).HasForeignKey(x => x.ProgramIdentifier);
            HasRequired(x => x.Group).WithMany(x => x.ProgramGroupEnrollments).HasForeignKey(x => x.GroupIdentifier);
        }
    }
}
