using System;
using System.Data.Entity;
using System.Linq;

using InSite.Application.QuizAttempts.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class TQuizAttemptStore : IQuizAttemptStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void Insert(TQuizAttempt attempt)
        {
            using (var db = CreateContext())
            {
                db.TQuizAttempts.Add(attempt);
                db.SaveChanges();
            }
        }

        public void Update(TQuizAttempt attempt)
        {
            using (var db = CreateContext())
            {
                db.Entry(attempt).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public bool Delete(Guid attemptId)
        {
            using (var db = CreateContext())
            {
                var entity = db.TQuizAttempts.Where(x => x.AttemptIdentifier == attemptId).FirstOrDefault();
                if (entity == null)
                    return false;

                db.TQuizAttempts.Remove(entity);
                db.SaveChanges();
            }

            return true;
        }

        public bool DeleteByQuizId(Guid quizId)
        {
            using (var db = CreateContext())
            {
                var entities = db.TQuizAttempts.Where(x => x.QuizIdentifier == quizId).ToArray();
                if (entities.IsNotEmpty())
                    return false;

                db.TQuizAttempts.RemoveRange(entities);
                db.SaveChanges();
            }

            return true;
        }
    }
}
