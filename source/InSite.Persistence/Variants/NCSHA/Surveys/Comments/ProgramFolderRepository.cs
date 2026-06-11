using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contents.Read;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class ProgramFolderRepository
    {
        #region INSERT

        public static int InsertFolder()
        {
            var entity = new ProgramFolder { Created = DateTime.UtcNow };

            using (var db = new InternalDbContext())
            {
                db.ProgramFolders.Add(entity);
                db.SaveChanges();
            }

            return entity.ProgramFolderId;
        }

        public static void InsertComment(int folderId, Guid commentId)
        {
            var entity = new ProgramFolderComment { ProgramFolderId = folderId, CommentIdentifier = commentId };

            using (var db = new InternalDbContext())
            {
                db.ProgramFolderComments.Add(entity);
                db.SaveChanges();
            }
        }

        #endregion

        #region SELECT

        public static List<VComment> SelectForFolderViewer(int folderId)
        {
            using (var db = new InternalDbContext())
            {
                return db.ProgramFolderComments
                    .Where(x => x.ProgramFolderId == folderId)
                    .Join(db.VComments, a => a.CommentIdentifier, b => b.CommentIdentifier, (a, b) => b)
                    .OrderByDescending(x => x.CommentPosted)
                    .ToList();
            }
        }

        #endregion
    }
}
