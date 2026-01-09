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
                            AchievementLabel = b.AchievementLabel,
                            AchievementTitle = b.AchievementTitle
                        }
                    ).ToList();
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
