using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class ProgramFolderCommentMapping : EntityTypeConfiguration<ProgramFolderComment>
    {
        public ProgramFolderCommentMapping() : this("custom_ncsha")
        {
        }

        public ProgramFolderCommentMapping(string schema)
        {
            ToTable(schema + ".ProgramFolderComment");
            HasKey(x => x.CommentIdentifier);

            Property(x => x.CommentIdentifier).IsRequired();
        }
    }
}