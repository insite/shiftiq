using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardOrganizationConfiguration : EntityTypeConfiguration<QStandardOrganization>
    {
        public QStandardOrganizationConfiguration() : this("standard") { }

        public QStandardOrganizationConfiguration(string schema)
        {
            ToTable("QStandardOrganization", schema);
            HasKey(x => new { x.StandardIdentifier, x.ConnectedOrganizationIdentifier });
        }
    }
}
