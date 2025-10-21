using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TCandidateCommentConfiguration : EntityTypeConfiguration<VCandidateComment>
    {
        public TCandidateCommentConfiguration() : this("jobs") { }

        public TCandidateCommentConfiguration(string schema)
        {
            ToTable(schema + ".VCandidateComment");
            HasKey(x => new { x.CommentIdentifier });
            Property(x => x.AuthorUserIdentifier).IsOptional();
            Property(x => x.CandidateUserIdentifier).IsOptional();
            Property(x => x.CommentIdentifier).IsRequired();
            Property(x => x.CommentIsFlagged).IsRequired();
            Property(x => x.CommentModified).IsRequired();
            Property(x => x.CommentText).IsRequired().IsUnicode(true);

            HasOptional(a => a.Author).WithMany(b => b.AuthorCandidateComments).HasForeignKey(c => c.AuthorUserIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Candidate).WithMany(b => b.CandidateComments).HasForeignKey(c => c.CandidateUserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
