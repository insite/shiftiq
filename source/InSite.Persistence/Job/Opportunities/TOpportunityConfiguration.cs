using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TOpportunityConfiguration : EntityTypeConfiguration<TOpportunity>
    {
        public TOpportunityConfiguration() : this("jobs") { }

        public TOpportunityConfiguration(string schema)
        {
            ToTable(schema + ".TOpportunity");
            HasKey(x => new { x.OpportunityIdentifier });
            Property(x => x.ApplicationDeadline).IsOptional();
            Property(x => x.ApplicationEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.ApplicationOpen).IsOptional();
            Property(x => x.ApplicationRequirements).IsOptional().IsUnicode(false).HasMaxLength(300);
            Property(x => x.ApplicationRequiresLetter).IsOptional();
            Property(x => x.ApplicationRequiresResume).IsOptional();
            Property(x => x.ApplicationWebSiteUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.DepartmentGroupIdentifier).IsOptional();
            Property(x => x.EmployerGroupDescription).IsOptional().IsUnicode(false).HasMaxLength(1400);
            Property(x => x.EmployerGroupIdentifier).IsOptional();
            Property(x => x.EmployerGroupName).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.EmployerUserIdentifier).IsRequired();
            Property(x => x.EmploymentStart).IsOptional();
            Property(x => x.EmploymentType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.JobAttachmentUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.JobDescription).IsOptional().IsUnicode(false);
            Property(x => x.JobLevel).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.JobTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LocationDescription).IsOptional().IsUnicode(false).HasMaxLength(800);
            Property(x => x.LocationName).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.LocationType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.OpportunityIdentifier).IsRequired();
            Property(x => x.OpportunityStatusItemIndentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.SalaryMaximum).IsOptional();
            Property(x => x.SalaryMinimum).IsOptional();
            Property(x => x.SalaryOther).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.WhenArchived).IsOptional();
            Property(x => x.WhenClosed).IsOptional();
            Property(x => x.WhenCreated).IsRequired();
            Property(x => x.WhenModified).IsOptional();
            Property(x => x.WhenPublished).IsOptional();

            HasOptional(a => a.OccupationStandard).WithMany(b => b.Opportunities).HasForeignKey(c => c.OccupationStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
