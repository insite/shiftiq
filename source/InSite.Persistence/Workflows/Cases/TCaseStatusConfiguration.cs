using System.Data.Entity.ModelConfiguration;

using InSite.Application.Cases.Read;

namespace InSite.Persistence
{
    public class TCaseStatusConfiguration : EntityTypeConfiguration<TCaseStatus>
    {
        public TCaseStatusConfiguration() : this("workflow") { }

        public TCaseStatusConfiguration(string schema)
        {
            ToTable("TCaseStatus", schema);
            HasKey(x => new { x.StatusIdentifier });

            Property(x => x.CaseType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.StatusCategory).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.ReportCategory).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.StatusIdentifier).IsRequired();
            Property(x => x.StatusName).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.StatusDescription).IsUnicode(false).HasMaxLength(200);
        }
    }
}
