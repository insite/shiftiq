using System;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Contents.Read;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class TCandidateCommentStore
    {
        private static QComment Convert(VCandidateComment comment)
        {
            return new QComment
            {
                AuthorUserIdentifier = comment.AuthorUserIdentifier ?? UserIdentifiers.Someone,
                AuthorUserName = comment.AuthorName ?? UserNames.Someone,
                CommentIdentifier = comment.CommentIdentifier,
                CommentPosted = comment.CommentModified,
                CommentText = comment.CommentText,
                ContainerType = "Person",
                ContainerSubtype = "Candidate",
                OrganizationIdentifier = comment.OrganizationIdentifier,
                TopicUserIdentifier = comment.CandidateUserIdentifier ?? UserIdentifiers.Someone
            };
        }

        public static void Insert(VCandidateComment comment)
        {
            var entity = Convert(comment);

            if (entity.CommentIdentifier == Guid.Empty)
                entity.CommentIdentifier = Guid.NewGuid();

            using (var db = new InternalDbContext())
            {
                db.QComments.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(VCandidateComment comment)
        {
            using (var db = new InternalDbContext())
            {
                var entity = Convert(comment);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid comment)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QComments.FirstOrDefault(x => x.CommentIdentifier == comment);
                if (entity == null)
                    return;

                db.QComments.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
