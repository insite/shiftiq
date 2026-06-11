using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class DepartmentProfileCompetencyConfiguration : EntityTypeConfiguration<DepartmentProfileCompetency>
    {
        public DepartmentProfileCompetencyConfiguration() : this("standards") { }

        public DepartmentProfileCompetencyConfiguration(string schema)
        {
            ToTable(schema + ".DepartmentProfileCompetency");
            HasKey(x => new { x.DepartmentIdentifier,x.ProfileStandardIdentifier,x.CompetencyStandardIdentifier });
            Property(x => x.CompetencyStandardIdentifier).IsRequired();
            Property(x => x.DepartmentIdentifier).IsRequired();
            Property(x => x.IsCritical).IsRequired();
            Property(x => x.LifetimeMonths).IsOptional();
            Property(x => x.ProfileStandardIdentifier).IsRequired();

            HasRequired(a => a.Department).WithMany(b => b.ProfileCompetencies).HasForeignKey(a => a.DepartmentIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Profile).WithMany(b => b.DepartmentProfiles).HasForeignKey(a => a.ProfileStandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Competency).WithMany(b => b.DepartmentCompetencies).HasForeignKey(a => a.CompetencyStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
