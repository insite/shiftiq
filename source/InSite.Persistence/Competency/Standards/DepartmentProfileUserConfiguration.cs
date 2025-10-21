using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class DepartmentProfileUserConfiguration : EntityTypeConfiguration<DepartmentProfileUser>
    {
        public DepartmentProfileUserConfiguration() : this("standards") { }

        public DepartmentProfileUserConfiguration(string schema)
        {
            ToTable(schema + ".DepartmentProfileUser");
            HasKey(x => new { x.DepartmentIdentifier,x.ProfileStandardIdentifier,x.UserIdentifier });

            HasRequired(a => a.Department).WithMany(b => b.ProfileUsers).HasForeignKey(a => a.DepartmentIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Profile).WithMany(b => b.DepartmentUsers).HasForeignKey(a => a.ProfileStandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.User).WithMany(b => b.DepartmentProfiles).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);

            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.IsInProgress).IsRequired();
            Property(x => x.IsPrimary).IsRequired();
            Property(x => x.IsRecommended).IsRequired();
            Property(x => x.IsRequired).IsRequired();
            Property(x => x.ProfileStandardIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}
