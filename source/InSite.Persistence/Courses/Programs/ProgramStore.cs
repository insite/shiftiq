using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class ProgramStore
    {
        public static void Update(TProgram list, Guid modifiedBy)
        {
            list.Modified = DateTimeOffset.UtcNow;
            list.ModifiedBy = modifiedBy;

            using (var db = new InternalDbContext())
            {
                db.Entry(list).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Insert(TProgram list, Guid createdBy)
        {
            list.Created = DateTimeOffset.UtcNow;
            list.CreatedBy = createdBy;
            list.Modified = list.Created;
            list.ModifiedBy = list.CreatedBy;

            using (var db = new InternalDbContext())
            {
                db.TPrograms.Add(list);
                db.SaveChanges();
            }
        }

        public static void Delete(Guid programIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var program = db.TPrograms.FirstOrDefault(x => x.ProgramIdentifier == programIdentifier);
                var programEnrollments = db.TProgramEnrollments.Where(x => x.ProgramIdentifier == programIdentifier).ToList();
                var programTasks = db.TTasks.Where(x => x.ProgramIdentifier == programIdentifier).ToList();

                List<TPrerequisite> programPrerequisites = new List<TPrerequisite>();
                List<TTaskEnrollment> programTaskEnrollments = new List<TTaskEnrollment>();

                foreach (var task in programTasks)
                {
                    programPrerequisites.AddRange(db.TPrerequisites.Where(x => x.ObjectIdentifier == task.TaskIdentifier).ToList());
                    programTaskEnrollments.AddRange(db.TTaskEnrollments.Where(x=>x.TaskIdentifier == task.TaskIdentifier).ToList());
                }

                if(programPrerequisites.Count > 0)
                    db.TPrerequisites.RemoveRange(programPrerequisites);

                if (programTaskEnrollments.Count > 0)
                    db.TTaskEnrollments.RemoveRange(programTaskEnrollments);

                if (programEnrollments.Count > 0)
                    db.TProgramEnrollments.RemoveRange(programEnrollments);

                if (programTasks.Count > 0)
                    db.TTasks.RemoveRange(programTasks);

                db.TPrograms.Remove(program);

                db.SaveChanges();
            }
        }

        public static void InsertGroupEnrollment(Guid organizationId, Guid programId, Guid groupId, Guid creatorUserId)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TProgramGroupEnrollments.FirstOrDefault(x => x.ProgramIdentifier == programId && x.GroupIdentifier == groupId);
                if (existing != null)
                    return;

                var enrollment = new TProgramGroupEnrollment
                {
                    ProgramGroupEnrollmentIdentifier = UniqueIdentifier.Create(),
                    OrganizationIdentifier = organizationId,
                    GroupIdentifier = groupId,
                    ProgramIdentifier = programId,
                    Created = DateTimeOffset.UtcNow,
                    CreatedBy = creatorUserId,
                };

                db.TProgramGroupEnrollments.Add(enrollment);
                db.SaveChanges();
            }
        }

        public static void DeleteGroupEnrollment(Guid programId, Guid groupId)
        {
            using (var db = new InternalDbContext())
            {
                var enrollment = db.TProgramGroupEnrollments.FirstOrDefault(x => x.ProgramIdentifier == programId && x.GroupIdentifier == groupId);
                if (enrollment == null)
                    return;

                db.TProgramGroupEnrollments.Remove(enrollment);
                db.SaveChanges();
            }
        }

        public static void InsertEnrollment(Guid organization, Guid program, Guid learner, Guid creator, DateTimeOffset? completion = null)
        {
            using (var db = new InternalDbContext())
            {
                var enrollment = db.TProgramEnrollments.FirstOrDefault(x => x.ProgramIdentifier == program && x.LearnerUserIdentifier == learner);
                if (enrollment != null)
                    return;

                enrollment = new TProgramEnrollment
                {
                    Created = DateTimeOffset.UtcNow,
                    ProgressStarted = DateTimeOffset.UtcNow,
                    CreatedBy = creator,
                    EnrollmentIdentifier = UniqueIdentifier.Create(),
                    LearnerUserIdentifier = learner,
                    OrganizationIdentifier = organization,
                    ProgramIdentifier = program,
                    ProgressCompleted = completion
                };

                db.TProgramEnrollments.Add(enrollment);
                db.SaveChanges();
            }
        }

        public static void DeleteEnrollment(Guid program, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                var enrollment = db.TProgramEnrollments.FirstOrDefault(x => x.ProgramIdentifier == program && x.LearnerUserIdentifier == user);
                if (enrollment == null)
                    return;

                db.TProgramEnrollments.Remove(enrollment);
                db.SaveChanges();
            }
        }

        public static void UpdateEnrollmentCompletionDate(Guid program, Guid user, DateTimeOffset? completion)
        {
            using (var db = new InternalDbContext())
            {
                var enrollment = db.TProgramEnrollments.FirstOrDefault(x => x.ProgramIdentifier == program && x.LearnerUserIdentifier == user);
                if (enrollment == null)
                    return;

                enrollment.ProgressCompleted = completion;
                db.Entry(enrollment).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        public static void RemoveEnrollmentTaskCompletionDate(Guid user, Guid obnjectId)
        {
            using (var db = new InternalDbContext())
            {
                var enrollmentsJournalSetupsObjects = db.QJournalSetups.AsNoTracking().Where(x => x.AchievementIdentifier == obnjectId).Select(x => x.JournalSetupIdentifier).ToList();
                var enrollmentsGradeBookObjects = db.QGradebooks.AsNoTracking().Where(x=> x.AchievementIdentifier == obnjectId).Select(x=>x.GradebookIdentifier).ToList();

                List<Guid> enrollmentTasksObjIDs = new List<Guid>{obnjectId};
                enrollmentTasksObjIDs.AddRange(enrollmentsJournalSetupsObjects);
                enrollmentTasksObjIDs.AddRange(enrollmentsGradeBookObjects);

                foreach(var enrollmentTaskObjectId in enrollmentTasksObjIDs)
                {
                    var enrollmentTask = db.TTaskEnrollments
                        .Where(x => x.LearnerUserIdentifier == user &&
                                    x.ObjectIdentifier == enrollmentTaskObjectId).FirstOrDefault();

                    if (enrollmentTask != null)
                    {
                        enrollmentTask.ProgressCompleted = null;
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void IncreaseMessageStalledSentCount(Guid program, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TProgramEnrollments.Where(x => x.ProgramIdentifier == program && x.LearnerUserIdentifier == user).FirstOrDefault();
                if (entity != null)
                {
                    entity.MessageStalledSentCount++;
                    db.SaveChanges();
                }
            }
        }

        public static void RemoveCompletionTaskIdFromProgram(Guid program, Guid task)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TPrograms.Where(x => x.ProgramIdentifier == program && x.CompletionTaskIdentifier == task).FirstOrDefault();
                if (entity != null)
                {
                    entity.CompletionTaskIdentifier = null;
                    db.SaveChanges();
                }
            }
        }
    }
}