using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VReportConfiguration : EntityTypeConfiguration<VReport>
    {
        public VReportConfiguration() : this("reports") { }

        public VReportConfiguration(string schema)
        {
            ToTable(schema + ".VReport");
            HasKey(x => new { x.ReportIdentifier });

            Property(x => x.ReportIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.ReportType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ReportTitle).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.ReportData).IsRequired().IsUnicode(false);
            Property(x => x.ReportDescription).IsOptional().IsUnicode(false).HasMaxLength(300);

            Property(x => x.Created).IsOptional();
            Property(x => x.CreatedBy).IsOptional();
            Property(x => x.Modified).IsOptional();
            Property(x => x.ModifiedBy).IsOptional();

            Property(x => x.CreatedByFullName).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.ModifiedByFullName).IsOptional().IsUnicode(false).HasMaxLength(120);
        }
    }
}
