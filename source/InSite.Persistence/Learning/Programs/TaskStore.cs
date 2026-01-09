using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TaskStore
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        public static void Insert(List<TTask> items)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var item in items)
                    if (!db.TTasks.Where(x => x.ObjectIdentifier == item.ObjectIdentifier && x.ProgramIdentifier == item.ProgramIdentifier).Any())
                        db.TTasks.Add(item);

                db.SaveChanges();
            }
        }

        public static void InsertPrerequisite(TPrerequisite prerequisite)
        {
            using (var db = new InternalDbContext())
            {
                db.TPrerequisites.Add(prerequisite);
                db.SaveChanges();
            }
        }

        public static void UpdateTaskSequence(Guid taskId, int sequence)
        {
            using (var db = new InternalDbContext())
            {
                var task = db.TTasks.FirstOrDefault(x => x.TaskIdentifier == taskId);
                if (task == null)
                    return;

                task.TaskSequence = sequence;
                db.Entry(task).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        public static void Update(TTask item)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Update(List<TTask> items)
        {
            if (items.Count == 0)
                return;

            var achievementListIdentifier = items.FirstOrDefault().ProgramIdentifier;

            using (var db = new InternalDbContext())
            {
                var original = db.TTasks.Where(x => x.ProgramIdentifier == achievementListIdentifier);

                foreach (var entity in items)
                {
                    var originalEntity = original.FirstOrDefault(x => x.ObjectIdentifier == entity.ObjectIdentifier);

                    if (originalEntity != null)
                    {
                        if (originalEntity.TaskLifetimeMonths != entity.TaskLifetimeMonths
                            || originalEntity.TaskIsRequired != entity.TaskIsRequired
                            || originalEntity.TaskIsPlanned != entity.TaskIsPlanned
                            )
                        {
                            originalEntity.TaskLifetimeMonths = entity.TaskLifetimeMonths;
                            originalEntity.TaskIsRequired = entity.TaskIsRequired;
                            originalEntity.TaskIsPlanned = entity.TaskIsPlanned;
                            db.Entry(originalEntity).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        db.TTasks.Add(entity);
                    }
                }

                db.SaveChanges();
            }
        }

        public static void Delete(Guid achievementListIdentifier, IEnumerable<Guid> achievements)
        {
            using (var db = new InternalDbContext())
            {
                var items = db.TTasks
                    .Where(x => x.ProgramIdentifier == achievementListIdentifier && achievements.Contains(x.ObjectIdentifier))
                    .ToList();

                db.TTasks.RemoveRange(items);
                db.SaveChanges();
            }
        }

        public static Guid Insert(Guid organization, Guid program, Guid objectIdentifier, string objectType, string taskCompletionRequirement)
        {
            using (var db = CreateContext())
            {
                var join = db.TTasks
                    .FirstOrDefault(x => x.ProgramIdentifier == program && x.ObjectIdentifier == objectIdentifier);

                if (join == null)
                {
                    var id = UniqueIdentifier.Create();
                    join = new TTask
                    {
                        TaskIdentifier = id,
                        OrganizationIdentifier = organization,
                        ProgramIdentifier = program,
                        ObjectIdentifier = objectIdentifier,
                        ObjectType = objectType,
                        TaskCompletionRequirement = taskCompletionRequirement,
                    };
                    db.TTasks.Add(join);
                    db.SaveChanges();
                    return id;
                }
                return join.TaskIdentifier;
            }
        }

        public static TTask Delete(Guid programIdentifier, Guid objectIdentifier)
        {
            using (var db = CreateContext())
            {
                var task = db.TTasks
                    .FirstOrDefault(x => x.ProgramIdentifier == programIdentifier && x.ObjectIdentifier == objectIdentifier);

                if (task != null)
                {
                    db.TTasks.Remove(task);
                    db.SaveChanges();
                }

                return task;
            }
        }

        public static TTask[] EnrollUserToProgramTasks(Guid organization, Guid program)
        {
            var learners = new Guid[0];

            using (var db = CreateContext())
            {
                learners = db.TProgramEnrollments
                    .Where(x => x.ProgramIdentifier == program)
                    .Select(x => x.LearnerUserIdentifier)
                    .ToArray();
            }

            var result = new List<TTask>();

            foreach (var learner in learners)
            {
                var tasks = EnrollUserToProgramTasks(organization, program, learner);
                if (tasks != null)
                    result.AddRange(tasks);
            }

            return result.ToArray();
        }

        public static TTask[] EnrollUserToProgramTasks(Guid organizationIdentifier, Guid programIdentifier, Guid userIdentifier)
        {
            using (var db = CreateContext())
            {
                var tasks = db.TTasks
                    .Where(x => x.ProgramIdentifier == programIdentifier).ToArray();

                if (tasks == null || tasks.Length == 0)
                    return null;

                var completedTasks = ProgramSearch1.GetProgramTaskCompletionForUser(programIdentifier, userIdentifier);

                foreach (var task in tasks)
                    EnrollUserToProgramTask(organizationIdentifier, task.TaskIdentifier, userIdentifier, task.ObjectIdentifier, completedTasks);

                return tasks.ToArray();
            }
        }

        public static void EnrollUserToProgramTask(Guid organizationIdentifier, Guid taskIdentifier, Guid userIdentifier, Guid objectIdentifier, List<VTaskEnrollment> completedTasks = null)
        {
            using (var db = CreateContext())
            {
                var enrollment = db.TTaskEnrollments
                    .FirstOrDefault(x => x.TaskIdentifier == taskIdentifier && x.LearnerUserIdentifier == userIdentifier && x.OrganizationIdentifier == organizationIdentifier);

                if (enrollment == null)
                {
                    enrollment = new TTaskEnrollment
                    {
                        EnrollmentIdentifier = UniqueIdentifier.Create(),
                        OrganizationIdentifier = organizationIdentifier,
                        LearnerUserIdentifier = userIdentifier,
                        TaskIdentifier = taskIdentifier,
                        ObjectIdentifier = objectIdentifier
                    };

                    db.TTaskEnrollments.Add(enrollment);
                }

                MarkTaskAsCompleted(enrollment, DateTimeOffset.UtcNow, completedTasks);

                db.SaveChanges();
            }
        }

        private static void MarkTaskAsCompleted(TTaskEnrollment enrollment, DateTimeOffset completed, List<VTaskEnrollment> completions)
        {
            var completion = completions.FirstOrDefault(x => x.TaskIdentifier == enrollment.TaskIdentifier && x.ObjectIdentifier == enrollment.ObjectIdentifier);

            if (completion == null)
                return;

            if (enrollment.ProgressStarted == null)
                enrollment.ProgressStarted = completed;

            if (enrollment.ProgressCompleted == null)
                enrollment.ProgressCompleted = completed;
        }

        public static TTask[] DeleteEnrollments(Guid organizationIdentifier, Guid programIdentifier, Guid userIdentifier)
        {
            using (var db = CreateContext())
            {
                var tasks = db.TTasks
                    .Where(x => x.ProgramIdentifier == programIdentifier).ToArray();

                if (tasks == null || tasks.Length == 0)
                    return null;

                foreach (var task in tasks)
                    DeleteEnrollmentProgramTask(organizationIdentifier, task.TaskIdentifier, userIdentifier);

                return tasks;
            }
        }

        public static void DeleteEnrollments(Guid organizationIdentifier, Guid taskIdentifier)
        {
            using (var db = CreateContext())
            {
                var enrollments = db.TTaskEnrollments
                    .Where(x => x.TaskIdentifier == taskIdentifier && x.OrganizationIdentifier == organizationIdentifier).ToArray();

                foreach (var enrollment in enrollments)
                    if (enrollment != null)
                    {
                        db.TTaskEnrollments.Remove(enrollment);
                        db.SaveChanges();
                    }
            }
        }

        private static void DeleteEnrollmentProgramTask(Guid organizationIdentifier, Guid taskIdentifier, Guid userIdentifier)
        {
            using (var db = CreateContext())
            {
                var enrolled = db.TTaskEnrollments
                    .FirstOrDefault(x => x.TaskIdentifier == taskIdentifier && x.LearnerUserIdentifier == userIdentifier && x.OrganizationIdentifier == organizationIdentifier);

                if (enrolled != null)
                {
                    db.TTaskEnrollments.Remove(enrolled);
                    db.SaveChanges();
                }
            }
        }

        public static void CompleteTaskEnrollementFoLearner(Guid value, Guid taskId, Guid objectIdentifier, Guid userIdentifier)
        {
            using (var db = CreateContext())
            {
                var enrolled = db.TTaskEnrollments
                    .FirstOrDefault(x => x.TaskIdentifier == taskId && x.LearnerUserIdentifier == userIdentifier && x.ObjectIdentifier == objectIdentifier);

                if (enrolled != null)
                {
                    enrolled.ProgressStarted = enrolled.ProgressCompleted = DateTimeOffset.UtcNow;
                    db.SaveChanges();
                }
            }
        }
    }
}