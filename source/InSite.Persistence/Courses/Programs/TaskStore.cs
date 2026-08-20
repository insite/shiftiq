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

        public static void Insert(List<TTask> items, bool cascadeToLearners = true)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var item in items)
                    if (!db.TTasks.Where(x => x.ObjectIdentifier == item.ObjectIdentifier && x.ProgramIdentifier == item.ProgramIdentifier).Any())
                        db.TTasks.Add(item);

                db.SaveChanges();
            }

            foreach (var programId in items.Select(x => x.ProgramIdentifier).Distinct())
                ProgramContainmentCascade.CascadeToChildren(programId);

            if (cascadeToLearners)
                foreach (var group in items.Where(IsAchievement).GroupBy(x => x.ProgramIdentifier))
                {
                    var organizationIdentifier = group
                        .Select(x => x.OrganizationIdentifier)
                        .FirstOrDefault(x => x != Guid.Empty);

                    if (organizationIdentifier == Guid.Empty)
                        organizationIdentifier = GetOrganizationIdentifier(group.Key);

                    ProgramTaskCascade.Synchronize(organizationIdentifier, group.Key, group.Select(x => x.ObjectIdentifier));
                }
        }

        private static bool IsAchievement(TTask task) =>
            task.ObjectType == ProgramTaskCascade.Achievement;

        /// <summary>
        /// Task rows do not always have an organization, and one that does not drops out of any
        /// organization-scoped query. In this scenario, read it from the program, which always has
        /// an organization assigned.
        /// </summary>
        private static Guid GetOrganizationIdentifier(Guid programIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.TPrograms
                    .Where(x => x.ProgramIdentifier == programIdentifier)
                    .Select(x => x.OrganizationIdentifier)
                    .FirstOrDefault();
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

        public static void UpdateOrInsert(List<TTask> items, bool cascadeToLearners = true)
        {
            if (items.Count == 0)
                return;

            var achievementListIdentifier = items.FirstOrDefault().ProgramIdentifier;
            var organizationIdentifier = Guid.Empty;
            var changed = new List<Guid>();

            using (var db = new InternalDbContext())
            {
                organizationIdentifier = db.TPrograms
                    .Where(x => x.ProgramIdentifier == achievementListIdentifier)
                    .Select(x => x.OrganizationIdentifier)
                    .FirstOrDefault();

                var original = db.TTasks.Where(x => x.ProgramIdentifier == achievementListIdentifier);

                foreach (var entity in items)
                {
                    var originalEntity = original.FirstOrDefault(x => x.ObjectIdentifier == entity.ObjectIdentifier);

                    if (originalEntity != null)
                    {
                        // Inherited task settings are derived from the parent programs;
                        // they can only change through the containment cascade.
                        if (originalEntity.TaskIsInherited)
                            continue;

                        if (originalEntity.TaskLifetimeMonths != entity.TaskLifetimeMonths
                            || originalEntity.TaskIsRequired != entity.TaskIsRequired
                            || originalEntity.TaskIsPlanned != entity.TaskIsPlanned
                            )
                        {
                            originalEntity.TaskLifetimeMonths = entity.TaskLifetimeMonths;
                            originalEntity.TaskIsRequired = entity.TaskIsRequired;
                            originalEntity.TaskIsPlanned = entity.TaskIsPlanned;
                            db.Entry(originalEntity).State = EntityState.Modified;

                            if (IsAchievement(originalEntity))
                                changed.Add(originalEntity.ObjectIdentifier);
                        }
                    }
                    else
                    {
                        // A task row with no organization falls out of an organization-scoped query,
                        // including the learner cascade, so never let one through.
                        if (entity.OrganizationIdentifier == Guid.Empty)
                            entity.OrganizationIdentifier = organizationIdentifier;

                        db.TTasks.Add(entity);

                        if (IsAchievement(entity))
                            changed.Add(entity.ObjectIdentifier);
                    }
                }

                db.SaveChanges();
            }

            ProgramContainmentCascade.CascadeToChildren(achievementListIdentifier);

            if (cascadeToLearners && changed.Count > 0)
                ProgramTaskCascade.Synchronize(organizationIdentifier, achievementListIdentifier, changed);
        }

        public static void Delete(Guid achievementListIdentifier, IEnumerable<Guid> achievements, bool cascadeToLearners = true)
        {
            var deleted = false;
            var organizationIdentifier = Guid.Empty;
            var removed = new List<Guid>();

            using (var db = new InternalDbContext())
            {
                // Inherited tasks can only be removed by unlinking the parent program.
                var items = db.TTasks
                    .Where(x => x.ProgramIdentifier == achievementListIdentifier && achievements.Contains(x.ObjectIdentifier) && !x.TaskIsInherited)
                    .ToList();

                if (items.Count > 0)
                {
                    organizationIdentifier = items
                        .Select(x => x.OrganizationIdentifier)
                        .FirstOrDefault(x => x != Guid.Empty);

                    removed = items.Where(IsAchievement).Select(x => x.ObjectIdentifier).ToList();

                    db.TTasks.RemoveRange(items);
                    db.SaveChanges();
                    deleted = true;
                }
            }

            if (!deleted)
                return;

            ProgramContainmentCascade.CascadeToChildren(achievementListIdentifier);

            // Run after the rows are gone so the merge sees only the programs that still claim the
            // achievement. A learner enrolled in one of those keeps their setting.
            if (cascadeToLearners && removed.Count > 0)
            {
                if (organizationIdentifier == Guid.Empty)
                    organizationIdentifier = GetOrganizationIdentifier(achievementListIdentifier);

                ProgramTaskCascade.Synchronize(organizationIdentifier, achievementListIdentifier, removed);
            }
        }

        public static Guid Insert(Guid organization, Guid program, Guid objectIdentifier, string objectType, string taskCompletionRequirement, bool cascadeToLearners = true)
        {
            var inserted = false;
            Guid taskIdentifier;

            using (var db = CreateContext())
            {
                var join = db.TTasks
                    .FirstOrDefault(x => x.ProgramIdentifier == program && x.ObjectIdentifier == objectIdentifier);

                if (join == null)
                {
                    var id = UniqueIdentifier.Create();
                    var isAchievement = objectType == ProgramTaskCascade.Achievement;

                    join = new TTask
                    {
                        TaskIdentifier = id,
                        OrganizationIdentifier = organization,
                        ProgramIdentifier = program,
                        ObjectIdentifier = objectIdentifier,
                        ObjectType = objectType,
                        TaskCompletionRequirement = taskCompletionRequirement,

                        // A new achievement task is part of the curriculum from the moment it is
                        // added, so it starts planned and required. An administrator can relaxe it
                        // afterward on the program settings page.
                        TaskIsPlanned = isAchievement,
                        TaskIsRequired = isAchievement
                    };

                    ProgramTaskDefaults.Apply(join);

                    db.TTasks.Add(join);
                    db.SaveChanges();

                    inserted = true;
                }

                taskIdentifier = join.TaskIdentifier;
            }

            if (!inserted)
                return taskIdentifier;

            ProgramContainmentCascade.CascadeToChildren(program);

            if (cascadeToLearners && objectType == ProgramTaskCascade.Achievement)
                ProgramTaskCascade.Synchronize(organization, program, new[] { objectIdentifier });

            return taskIdentifier;
        }

        public static TTask Delete(Guid programIdentifier, Guid objectIdentifier, bool cascadeToLearners = true)
        {
            TTask task;

            using (var db = CreateContext())
            {
                task = db.TTasks
                    .FirstOrDefault(x => x.ProgramIdentifier == programIdentifier && x.ObjectIdentifier == objectIdentifier);

                // Inherited tasks can only be removed by unlinking the parent program.
                // Returning null tells the caller nothing was deleted, so it must not
                // run its enrollment and content cleanup.
                if (task == null || task.TaskIsInherited)
                    return null;

                db.TTasks.Remove(task);
                db.SaveChanges();
            }

            ProgramContainmentCascade.CascadeToChildren(programIdentifier);

            if (cascadeToLearners && IsAchievement(task))
            {
                var organizationIdentifier = task.OrganizationIdentifier == Guid.Empty
                    ? GetOrganizationIdentifier(programIdentifier)
                    : task.OrganizationIdentifier;

                ProgramTaskCascade.Synchronize(organizationIdentifier, programIdentifier, new[] { objectIdentifier });
            }

            return task;
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

        public static void CompleteTaskEnrollementForLearner(Guid taskId, Guid objectIdentifier, Guid userIdentifier)
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
