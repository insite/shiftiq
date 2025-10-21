using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCandidateUploadConfiguration : EntityTypeConfiguration<TCandidateUpload>
    {
        public TCandidateUploadConfiguration() : this("jobs") { }

        public TCandidateUploadConfiguration(string schema)
        {
            ToTable(schema + ".TCandidateUpload");
            HasKey(x => new { x.UploadIdentifier });
            Property(x => x.CandidateUserIdentifier).IsOptional();
            Property(x => x.UploadIdentifier).IsRequired();
            Property(x => x.UploadMime).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.UploadSize).IsRequired();
            Property(x => x.UploadType).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UploadName).IsRequired().IsUnicode(false).HasMaxLength(300);

            HasOptional(a => a.Candidate).WithMany(b => b.CandidateUploads).HasForeignKey(c => c.CandidateUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
