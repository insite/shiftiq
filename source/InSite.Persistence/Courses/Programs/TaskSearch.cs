using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public static class TaskSearch
    {
        private class AchievementListItemReadHelper : ReadHelper<TTask>
        {
            public static readonly AchievementListItemReadHelper Instance = new AchievementListItemReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TTask>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TTasks.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TTask SelectFirst(Expression<Func<TTask, bool>> filter,
            params Expression<Func<TTask, object>>[] includes)
        {
            return AchievementListItemReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<TTask> Select(
            Expression<Func<TTask, bool>> filter,
            params Expression<Func<TTask, object>>[] includes)
        {
            return AchievementListItemReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<TTask> Select(
            Expression<Func<TTask, bool>> filter,
            string sortExpression,
            params Expression<Func<TTask, object>>[] includes)
        {
            return AchievementListItemReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<TTask, T>> binder,
            Expression<Func<TTask, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementListItemReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<TTask, T>> binder,
            Expression<Func<TTask, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementListItemReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<TTask, bool>> filter)
        {
            return AchievementListItemReadHelper.Instance.Count(filter);
        }

        public static List<TaskSearchItem> SelectByProgram(Guid programIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTasks
                    .Where(x => x.ProgramIdentifier == programIdentifier)
                    .Join(db.QAchievements,
                        a => a.ObjectIdentifier,
                        b => b.AchievementIdentifier,
                        (a, b) => new TaskSearchItem
                        {
                            DepartmentIdentifier = a.Program.GroupIdentifier ?? Guid.Empty,
                            ProgramIdentifier = a.ProgramIdentifier,
                            AchievementIdentifier = a.ObjectIdentifier,
                            LifetimeMonths = a.TaskLifetimeMonths,
                            IsRequired = a.TaskIsRequired,
                            IsPlanned = a.TaskIsPlanned,
                            IsInherited = a.TaskIsInherited,
                            AchievementLabel = b.AchievementLabel,
                            AchievementTitle = b.AchievementTitle
                        }
                    ).ToList();
            }
        }

        /// <summary>
        /// Achievements the learner cannot safely have downgraded: any task that is
        /// required or planned in another program the learner is enrolled in.
        /// </summary>
        public static Guid[] SelectProtectedObjectIdentifiers(Guid organizationIdentifier, Guid learnerUserIdentifier, Guid excludeProgramIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTasks.AsNoTracking()
                    .Where(t => t.OrganizationIdentifier == organizationIdentifier
                        && t.ProgramIdentifier != excludeProgramIdentifier
                        && (t.TaskIsRequired || t.TaskIsPlanned)
                        && db.TProgramEnrollments.Any(e =>
                            e.ProgramIdentifier == t.ProgramIdentifier
                            && e.LearnerUserIdentifier == learnerUserIdentifier))
                    .Select(t => t.ObjectIdentifier)
                    .Distinct()
                    .ToArray();
            }
        }

        /// <summary>
        /// Every claim on the given achievements held by any program the learners of
        /// programIdentifier are enrolled in, including that program itself. The learner set is
        /// resolved inside the query, so a program with thousands of enrollments still costs one
        /// round trip and no large parameter list.
        /// </summary>
        public static List<LearnerTaskDemand> SelectLearnerDemands(Guid organizationIdentifier, Guid programIdentifier, Guid[] objectIdentifiers)
        {
            if (objectIdentifiers.Length == 0)
                return new List<LearnerTaskDemand>();

            using (var db = new InternalDbContext())
            {
                return db.TProgramEnrollments.AsNoTracking()
                    .Where(e => e.ProgramIdentifier == programIdentifier)
                    .Join(db.TProgramEnrollments.AsNoTracking(),
                        e => e.LearnerUserIdentifier,
                        o => o.LearnerUserIdentifier,
                        (e, o) => o)
                    .Join(db.TTasks.AsNoTracking()
                            .Where(t => t.OrganizationIdentifier == organizationIdentifier
                                && objectIdentifiers.Contains(t.ObjectIdentifier)),
                        o => o.ProgramIdentifier,
                        t => t.ProgramIdentifier,
                        (o, t) => new LearnerTaskDemand
                        {
                            LearnerUserIdentifier = o.LearnerUserIdentifier,
                            ObjectIdentifier = t.ObjectIdentifier,
                            ProgramIdentifier = t.ProgramIdentifier,
                            IsRequired = t.TaskIsRequired,
                            IsPlanned = t.TaskIsPlanned,
                            LifetimeMonths = t.TaskLifetimeMonths
                        })
                    .ToList();
            }
        }

        public static bool TaskExistInOtherProgram(Guid organizationIdentifier, Guid objectIdentifier, Guid userIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTaskEnrollments
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier && x.ObjectIdentifier == objectIdentifier && x.LearnerUserIdentifier == userIdentifier)
                    .ToList().Count > 0;
            }
        }

        public static TTaskEnrollment[] GetUserTaskEnrollments(Guid organizationIdentifier, Guid programIdentifier, Guid userIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTaskEnrollments
                    .AsNoTracking()
                    .Include(x => x.Task)
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier && x.LearnerUserIdentifier == userIdentifier
                            && x.Task.ProgramIdentifier == programIdentifier)
                    .ToArray();

            }
        }

        public static TTaskEnrollment[] GetUserTaskEnrollments(Guid organizationIdentifier, Guid userIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTaskEnrollments
                    .AsNoTracking()
                    .Include(x => x.Task)
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier && x.LearnerUserIdentifier == userIdentifier)
                    .ToArray();

            }
        }

        public static TTaskEnrollment[] GetProgramTaskEnrollments(Guid organizationIdentifier, Guid programIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTaskEnrollments
                    .AsNoTracking()
                    .Include(x => x.Task)
                    .Where(x => x.OrganizationIdentifier == organizationIdentifier
                            && x.Task.ProgramIdentifier == programIdentifier)
                    .ToArray();

            }
        }
    }
}
