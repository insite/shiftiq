using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QPeriodConfiguration : EntityTypeConfiguration<QPeriod>
    {
        public QPeriodConfiguration() : this("records") { }

        public QPeriodConfiguration(string schema)
        {
            ToTable(schema + ".QPeriod");
            HasKey(x => x.PeriodIdentifier);
            Property(x => x.PeriodIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.PeriodEnd).IsRequired();
            Property(x => x.PeriodName).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.PeriodStart).IsRequired();
        }
    }
}
