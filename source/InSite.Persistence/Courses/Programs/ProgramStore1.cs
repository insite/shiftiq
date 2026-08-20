using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class ProgramStore1 : IProgramStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        public List<TaskEnrollment> TaskCompleted(Guid learnerIdentifier, Guid organizationIdentifier, Guid objectIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entities = GetTaskEnrollments(learnerIdentifier, organizationIdentifier, objectIdentifier, db);

                if (entities == null)
                    GetTaskBasedOnAchievement(learnerIdentifier, organizationIdentifier, objectIdentifier);

                UpdateTaskEnrollments(db, entities);

                return entities.Select(e => new TaskEnrollment()
                {
                    LearnerIdentifier = e.LearnerUserIdentifier,
                    ObjectIdentifier = e.ObjectIdentifier,
                    OrganizationIdentifier = e.OrganizationIdentifier,
                    ProgramIdentifier = e.Task.ProgramIdentifier,
                    TaskIdentifier = e.Task.TaskIdentifier
                        
                }).ToList();
            }
        }

        public void TaskViewed(Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid ObjectIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TTaskEnrollments
                    .Where(x => x.OrganizationIdentifier == OrganizationIdentifier &&
                                x.LearnerUserIdentifier == LearnerIdentifier &&
                                x.ObjectIdentifier == ObjectIdentifier).FirstOrDefault();
                if (entity != null)
                {
                    entity.ProgressStarted = DateTimeOffset.Now;
                    db.SaveChanges();
                }
            }
        }

        public void ProgramCompleted(Guid ProgramIdentifier, Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid? AchievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TProgramEnrollments
                    .Where(x => x.OrganizationIdentifier == OrganizationIdentifier &&
                                x.LearnerUserIdentifier == LearnerIdentifier &&
                                x.ProgramIdentifier == ProgramIdentifier).FirstOrDefault();
                if (entity != null)
                {
                    entity.ProgressCompleted = DateTimeOffset.Now;
                    db.SaveChanges();

                    if (!AchievementIdentifier.HasValue)
                        return;

                    TaskCompleted(LearnerIdentifier, OrganizationIdentifier, AchievementIdentifier.Value);
                }
            }
        }

        private static TTaskEnrollment GetTaskEnrollment(Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid ObjectIdentifier, InternalDbContext db)
        {
            return db.TTaskEnrollments
                .Where(x => x.OrganizationIdentifier == OrganizationIdentifier &&
                            x.LearnerUserIdentifier == LearnerIdentifier &&
                            x.ObjectIdentifier == ObjectIdentifier).FirstOrDefault();
        }

        private static List<TTaskEnrollment> GetTaskEnrollments(Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid ObjectIdentifier, InternalDbContext db)
        {
            return db.TTaskEnrollments
                .AsQueryable()
                .Include(x => x.Task)
                .Where(x => x.OrganizationIdentifier == OrganizationIdentifier &&
                            x.LearnerUserIdentifier == LearnerIdentifier &&
                            x.ObjectIdentifier == ObjectIdentifier
                ).ToList();
        }

        private static void UpdateTaskEnrollment(InternalDbContext db, TTaskEnrollment entity)
        {
            if (entity != null)
            {
                entity.ProgressCompleted = DateTimeOffset.Now;
                db.SaveChanges();
            }
        }

        private static void UpdateTaskEnrollments(InternalDbContext db, List<TTaskEnrollment> entities)
        {
            if (entities == null || entities.Count == 0)
                return;

            foreach (TTaskEnrollment entity in entities)
                UpdateTaskEnrollment(db, entity);

            var learnerUserId = entities[0].LearnerUserIdentifier;

            var programIds = entities.Select(x => x.Task.ProgramIdentifier).Distinct();
            foreach (var programId in programIds)
            {
                db.Database.ExecuteSqlCommand("exec records.CompleteProgramWhenAllTasksCompleted @ProgramId, @LearnerUserId",
                    new SqlParameter("ProgramId", programId),
                    new SqlParameter("LearnerUserId", learnerUserId)
                );
            }
        }

        public static void CompleteProgramWhenAllTasksCompleted(Guid programId, Guid learnerUserId)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand("exec records.CompleteProgramWhenAllTasksCompleted @ProgramId, @LearnerUserId",
                    new SqlParameter("ProgramId", programId),
                    new SqlParameter("LearnerUserId", learnerUserId)
                );
            }
        }

        private static void GetTaskBasedOnAchievement(Guid LearnerIdentifier, Guid OrganizationIdentifier, Guid ObjectIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var tempLogbookIds = db.QJournalSetups.AsNoTracking().AsQueryable().Where(x => x.JournalSetupIdentifier == ObjectIdentifier).Select(y => y.AchievementIdentifier).ToList();
                foreach (var achievementId in tempLogbookIds)
                {
                    TTaskEnrollment entity = GetTaskEnrollment(LearnerIdentifier, OrganizationIdentifier, achievementId.Value, db);
                    UpdateTaskEnrollment(db, entity);
                }
            }
        }
    }
}
