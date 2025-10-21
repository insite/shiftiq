using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TCandidateExperienceStore
    {
        public static void Insert(TCandidateExperience entity)
        {
            if (entity.ExperienceIdentifier == Guid.Empty)
                entity.ExperienceIdentifier = Guid.NewGuid();

            using (var db = new InternalDbContext())
            {
                db.TCandidateExperiences.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(TCandidateExperience entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid experienceId)
        {
            using (var db = new InternalDbContext())
            {
                var experience = db.TCandidateExperiences.FirstOrDefault(x => x.ExperienceIdentifier == experienceId);
                if (experience != null)
                    db.TCandidateExperiences.Remove(experience);
                db.SaveChanges();
            }
        }

        public static void DeleteByUser(Guid user)
        {
            using (var db = new InternalDbContext())
            {
                var experiences = db.TCandidateExperiences.Where(x => x.UserIdentifier == user).ToList();
                db.TCandidateExperiences.RemoveRange(experiences);

                db.SaveChanges();
            }
        }
    }
}
