using System.Data.Entity.ModelConfiguration;

using InSite.Application.Resources.Read;

namespace InSite.Persistence
{
    public class VUploadConfiguration : EntityTypeConfiguration<VUpload>
    {
        public VUploadConfiguration() : this("resources") { }

        public VUploadConfiguration(string schema)
        {
            ToTable(schema + ".VUpload");
            HasKey(x => x.UploadIdentifier);

            Property(x => x.UploadType).IsRequired().IsUnicode(false).HasMaxLength(16);
            Property(x => x.AccessType).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.Title).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.Name).IsRequired().IsUnicode(false).HasMaxLength(256);
            Property(x => x.Description).IsUnicode(false);
            Property(x => x.NavigateUrl).IsUnicode(false).HasMaxLength(512);
            Property(x => x.ContainerType).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ContentFingerprint).IsUnicode(false).HasMaxLength(24);
            Property(x => x.ContentType).IsUnicode(false).HasMaxLength(32);
            Property(x => x.UploaderName).IsUnicode(false).HasMaxLength(148);
        }
    }
}
