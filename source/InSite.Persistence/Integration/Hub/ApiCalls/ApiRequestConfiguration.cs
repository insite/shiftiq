using System.Data.Entity.ModelConfiguration;

using InSite.Domain.Integration;

namespace InSite.Persistence
{
    public class ApiRequestConfiguration : EntityTypeConfiguration<ApiRequest>
    {
        public ApiRequestConfiguration() : this("reports") { }

        public ApiRequestConfiguration(string schema)
        {
            ToTable(schema + ".ApiRequest");
            HasKey(x => x.RequestIdentifier);

            Property(x => x.DeveloperEmail).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.DeveloperHostAddress).IsOptional().IsUnicode(false).HasMaxLength(15);
            Property(x => x.DeveloperName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ExecutionErrors).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.RequestContentData).IsOptional().IsUnicode(false);
            Property(x => x.RequestContentType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RequestHeaders).IsOptional().IsUnicode(false).HasMaxLength(5000);
            Property(x => x.RequestMethod).IsRequired().IsUnicode(false).HasMaxLength(6);
            Property(x => x.RequestStarted).IsRequired();
            Property(x => x.RequestStatus).IsRequired().IsUnicode(false).HasMaxLength(16);
            Property(x => x.RequestUri).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.RequestDirection).IsRequired().IsUnicode(false).HasMaxLength(3);
            Property(x => x.ResponseCompleted).IsOptional();
            Property(x => x.ResponseContentData).IsOptional();
            Property(x => x.ResponseContentType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ResponseStatusName).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ResponseStatusNumber).IsOptional();
            Property(x => x.ResponseTime).IsOptional();
            Property(x => x.ResponseLogIdentifier).IsOptional();
            Property(x => x.OrganizationIdentifier).IsOptional();
            Property(x => x.UserIdentifier).IsOptional();
            Property(x => x.ValidationErrors).IsOptional().IsUnicode(false);
            Property(x => x.ValidationStatus).IsRequired().IsUnicode(false).HasMaxLength(16);

            HasOptional(a => a.User).WithMany(b => b.ApiRequests).HasForeignKey(c => c.UserIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Organization).WithMany(b => b.ApiRequests).HasForeignKey(c => c.OrganizationIdentifier).WillCascadeOnDelete(false);
        }
    }
}