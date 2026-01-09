using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class ResourceCommentSummaryConfiguration : EntityTypeConfiguration<ResourceCommentSummary>
    {
        public ResourceCommentSummaryConfiguration() : this("resources") { }

        public ResourceCommentSummaryConfiguration(string schema)
        {
            ToTable(schema + ".ResourceCommentSummary");
            HasKey(x => new { x.CommentKey });

            Property(x => x.AuthorFirstName).IsUnicode(false).HasMaxLength(64);
            Property(x => x.AuthorLastName).IsUnicode(false).HasMaxLength(80);
            Property(x => x.AuthorName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.CommentBody).IsRequired().IsUnicode(false);
            Property(x => x.CommentTitle).IsRequired().IsUnicode(false).HasMaxLength(150);
            Property(x => x.ResourceType).IsRequired().IsUnicode(false).HasMaxLength(64);
        }
    }
}
