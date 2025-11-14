using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TCandidateEducationStore
    {
        public static void Insert(TCandidateEducation entity)
        {
            if (entity.EducationIdentifier == Guid.Empty)
                entity.EducationIdentifier = Guid.NewGuid();

            using (var db = new InternalDbContext())
            {
                db.TCandidateEducations.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(TCandidateEducation entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid educationIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TCandidateEducations.FirstOrDefault(x => x.EducationIdentifier == educationIdentifier);

                if (entity != null)
                {
                    db.TCandidateEducations.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public static void DeleteByUser(Guid user)
        {
            using (var db = new InternalDbContext())
            {
                var entities = db.TCandidateEducations.Where(x => x.UserIdentifier == user).ToList();

                db.TCandidateEducations.RemoveRange(entities);
                db.SaveChanges();
            }
        }
    }
}
