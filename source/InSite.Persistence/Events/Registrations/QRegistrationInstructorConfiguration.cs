using System.Data.Entity.ModelConfiguration;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence
{
    public class QRegistrationInstructorConfiguration : EntityTypeConfiguration<QRegistrationInstructor>
    {
        public QRegistrationInstructorConfiguration() : this("registrations") { }

        public QRegistrationInstructorConfiguration(string schema)
        {
            ToTable(schema + ".QRegistrationInstructor");
            HasKey(x => new { x.RegistrationIdentifier, x.InstructorIdentifier });

            HasRequired(a => a.Registration).WithMany(b => b.RegistrationInstructors).HasForeignKey(c => c.RegistrationIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Instructor).WithMany(b => b.RegistrationInstructors).HasForeignKey(c => new { c.OrganizationIdentifier, c.InstructorIdentifier }).WillCascadeOnDelete(false);

            Property(x => x.RegistrationIdentifier).IsRequired();
            Property(x => x.InstructorIdentifier).IsRequired();
        }
    }
}
