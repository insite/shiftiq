using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TScormRegistrationActivityConfiguration : EntityTypeConfiguration<TScormRegistrationActivity>
    {
        public TScormRegistrationActivityConfiguration()
        {
            HasRequired(a => a.Registration).WithMany(b => b.Activities).HasForeignKey(a => a.ScormRegistrationIdentifier).WillCascadeOnDelete(false);

            ToTable("TScormRegistrationActivity", "integration");
            HasKey(x => new { x.JoinIdentifier });

            Property(x => x.ActivityIdentifier).IsRequired();
            Property(x => x.CourseIdentifier).IsRequired();
            Property(x => x.GradeItemIdentifier);
            Property(x => x.JoinIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ScormRegistrationIdentifier).IsRequired();
        }
    }
}
