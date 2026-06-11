using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class UploadConfiguration : EntityTypeConfiguration<Upload>
    {
        public UploadConfiguration() : this("resources") { }

        public UploadConfiguration(string schema)
        {
            ToTable(schema + ".Upload");
            HasKey(x => new { x.UploadIdentifier });

            Property(x => x.ContainerIdentifier).IsRequired();
            Property(x => x.ContainerType).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ContentFingerprint).IsOptional().IsUnicode(false).HasMaxLength(24);
            Property(x => x.ContentSize).IsOptional();
            Property(x => x.ContentType).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.Description).IsOptional().IsUnicode(false).HasMaxLength(300);
            Property(x => x.Name).IsRequired().IsUnicode(false).HasMaxLength(500);
            Property(x => x.NavigateUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.Title).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.Uploaded).IsRequired();
            Property(x => x.Uploader).IsRequired();
            Property(x => x.UploadIdentifier).IsRequired();
            Property(x => x.UploadPrivacyScope).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.UploadType).IsRequired().IsUnicode(false).HasMaxLength(16);
        }
    }
}
