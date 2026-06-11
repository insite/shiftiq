using System;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Quizzes.Read;

namespace InSite.Persistence
{
    public class TQuizStore : IQuizStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        public void Insert(TQuiz quiz)
        {
            using (var db = CreateContext())
            {
                db.TQuizzes.Add(quiz);
                db.SaveChanges();
            }
        }

        public void Update(TQuiz quiz)
        {
            using (var db = CreateContext())
            {
                db.Entry(quiz).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public bool Delete(Guid quizId)
        {
            using (var db = CreateContext())
            {
                var entity = db.TQuizzes.Where(x => x.QuizIdentifier == quizId).FirstOrDefault();
                if (entity == null)
                    return false;

                db.TQuizzes.Remove(entity);
                db.SaveChanges();
            }

            return true;
        }
    }
}
