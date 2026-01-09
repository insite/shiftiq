using System;
using System.Data.Entity;

using InSite.Application.Contents.Read;

namespace InSite.Persistence
{
    public static class QCommentStore
    {
        #region DELETE

        public static void Delete(QComment comment)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(comment).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region INSERT

        public static QComment Insert(QComment entity)
        {
            using (var context = new InternalDbContext())
            {
                if (entity.CommentPosted == DateTimeOffset.MinValue)
                    entity.CommentPosted = DateTimeOffset.UtcNow;

                context.QComments.Add(entity);
                context.SaveChanges();
            }

            return entity;
        }

        #endregion

        #region UPDATE

        public static void Update(QComment entity)
        {
            using (var context = new InternalDbContext())
            {
                context.QComments.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        #endregion
    }
}
