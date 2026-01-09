using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class ProgramFolderComment
    {
        public int ProgramFolderId { get; set; }
        public int CommentId { get; set; }
        public Guid CommentIdentifier { get; set; }
    }
}