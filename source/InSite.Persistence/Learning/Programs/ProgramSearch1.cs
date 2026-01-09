using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class ProgramSearch1
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        private class AchievementListReadHelper : ReadHelper<TProgram>
        {
            public static readonly AchievementListReadHelper Instance = new AchievementListReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.TPrograms.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TProgram SelectFirst(Expression<Func<TProgram, bool>> filter,
            params Expression<Func<TProgram, object>>[] includes)
        {
            return AchievementListReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<TProgram> Select(
            Expression<Func<TProgram, bool>> filter,
            params Expression<Func<TProgram, object>>[] includes)
        {
            return AchievementListReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<TProgram> Select(
            Expression<Func<TProgram, bool>> filter,
            string sortExpression,
            params Expression<Func<TProgram, object>>[] includes)
        {
            return AchievementListReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<TProgram, T>> binder,
            Expression<Func<TProgram, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementListReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T[] Bind<T>(
            Expression<Func<TProgram, T>> binder,
            Expression<Func<TProgram, bool>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementListReadHelper.Instance.Bind(binder, filter, paging, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<TProgram, T>> binder,
            Expression<Func<TProgram, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return AchievementListReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<TProgram, bool>> filter)
        {
            return AchievementListReadHelper.Instance.Count(filter);
        }

        public static int Count(TProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static DataTable Select(TProgramFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Select(x => new
                    {
                        x.GroupIdentifier,
                        x.GroupName,
                        x.ProgramCode,
                        x.ProgramIdentifier,
                        x.ProgramName,
                        x.ProgramDescription,
                        x.CatalogName,
                        x.CategoryCount,
                        x.EnrollmentCount,
                        x.TaskCount

                    })
                    .OrderBy(x => x.ProgramName)
                    .ThenBy(x => x.ProgramCode)
                    .ThenBy(x => x.GroupName)
                    .ApplyPaging(filter)
                    .ToDataTable();
            }
        }

        private static IQueryable<VProgram> CreateQuery(TProgramFilter filter, InternalDbContext db)
        {
            var query = db.VPrograms
                .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.CatalogIdentifier.HasValue)
                query = query.Where(x => x.CatalogIdentifier == filter.CatalogIdentifier);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.GroupIdentifier == filter.GroupIdentifier);

            if (!string.IsNullOrEmpty(filter.ProgramName))
                query = query.Where(x => x.ProgramName.Contains(filter.ProgramName));

            if (!string.IsNullOrEmpty(filter.ProgramDescription))
                query = query.Where(x => x.ProgramDescription.Contains(filter.ProgramDescription));

            return query;
        }

        #region Program Enrollments

        public static int CountProgramUsers(VProgramEnrollmentFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public static List<VProgramEnrollment> GetProgramUsers(VProgramEnrollmentFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.ProgramCode)
                    .ThenBy(x => x.ProgramName)
                    .ThenBy(x => x.UserFullName)
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        public static List<TProgramEnrollment> GetUserProgramEnrollments(Guid userId, Guid[] organizationId)
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramEnrollments
                    .AsNoTracking()
                    .Where(x => x.LearnerUserIdentifier == userId && organizationId.Any(y => y == x.OrganizationIdentifier)
                    )
                    .ToList();
            }
        }

        public static List<TProgramEnrollment> SelectProgramUsersForMessages()
        {
            using (var db = new InternalDbContext())
            {
                return db.TProgramEnrollments
                    .AsNoTracking()
                    .Include(x => x.Program)
                    .Include(x => x.LearnerUser)
                    .Where(x => x.Program.NotificationStalledTriggerDay != null
                        && x.Program.NotificationStalledReminderLimit != null
                        && (x.Program.NotificationStalledAdministratorMessageIdentifier != null || x.Program.NotificationStalledLearnerMessageIdentifier != null)
                        && x.ProgressCompleted == null && x.ProgressStarted != null
                    )
                    .ToList();
            }
        }

        public static List<ProgramEnrollmentTaskCompletionCounterForUsers> GetProgramEnrollmentTaskCompletionCounterForUsers(Guid programId)
        {
            const string query = "exec records.GetProgramEnrollmentTaskCompletionCounterForUsers @ProgramIdentifier";

            using (var db = new InternalDbContext(false))
            {
                return db.Database
                    .SqlQuery<ProgramEnrollmentTaskCompletionCounterForUsers>(query, new SqlParameter("ProgramIdentifier", programId))
                    .ToList();
            }
        }

        public static DateTimeOffset? GetProgramLastTaskCompletionDate(Guid programId, Guid userId)
        {
            const string query = "exec records.GetProgramLastTaskCompletionDate @ProgramIdentifier, @UserIdentifier";

            using (var db = new InternalDbContext(false))
            {
                return db.Database
                    .SqlQuery<DateTimeOffset?>(query, new SqlParameter("ProgramIdentifier", programId), new SqlParameter("UserIdentifier", userId))
                    .FirstOrDefault();
            }
        }

        private static IQueryable<VProgramEnrollment> CreateQuery(VProgramEnrollmentFilter filter, InternalDbContext db)
        {
            var query = db.VProgramEnrollments.AsQueryable();

            if (filter.OrganizationIdentifier != Guid.Empty)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ProgramIdentifier.HasValue)
                query = query.Where(x => x.ProgramIdentifier == filter.ProgramIdentifier);

            if (filter.ProgramName.HasValue())
                query = query.Where(x => x.ProgramName.Contains(filter.ProgramName));

            if (filter.UserFullName.HasValue())
                query = query.Where(x => x.UserFullName.Contains(filter.UserFullName));

            if (filter.UserIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.UserIdentifiers.Contains(x.UserIdentifier));

            if (filter.CompletionDateIsEmpty)
                query = query.Where(x => x.ProgressCompleted == null);

            return query;
        }

        #endregion

        #region Program Tasks

        public static List<TTask> GetProgramTasks(TTaskFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = db.TTasks.AsNoTracking().AsQueryable();

                if (filter.ObjectIdentifier.HasValue)
                    query = query.Where(x => x.ObjectIdentifier == filter.ObjectIdentifier);

                if (filter.OrganizationIdentifiers != null && filter.OrganizationIdentifiers.Any())
                    query = query.Where(x => filter.OrganizationIdentifiers.Contains(x.OrganizationIdentifier));

                if (filter.ProgramIdentifier.HasValue)
                    query = query.Where(x => x.ProgramIdentifier == filter.ProgramIdentifier);

                if (filter.ExcludedTask.HasValue)
                    query = query.Where(x => x.TaskIdentifier != filter.ExcludedTask);

                if (filter.ExcludedObject.HasValue)
                    query = query.Where(x => x.ObjectIdentifier != filter.ExcludedObject);

                if (filter.ExcludeObjectTypes != null && filter.ExcludeObjectTypes.Length > 0)
                    query = query.Where(x => !filter.ExcludeObjectTypes.Contains(x.ObjectType));

                return query.ToList();
            }
        }

        public static TTask GetProgramTask(Guid taskId)
        {
            using (var db = CreateContext())
            {
                var query = db.TTasks.AsNoTracking().AsQueryable();
                return query.FirstOrDefault(x => x.TaskIdentifier == taskId);
            }
        }

        public static List<VTaskEnrollment> GetProgramTaskCompletionForUser(Guid program, Guid user)
        {
            const string query = "exec records.GetProgramTaskCompletionForUser @ProgramIdentifier, @UserIdentifier";

            using (var db = new InternalDbContext(false))
            {
                return db.Database
                    .SqlQuery<VTaskEnrollment>(query, new SqlParameter("ProgramIdentifier", program), new SqlParameter("UserIdentifier", user)).Where(x => x.CompletionCounter > 0)
                    .ToList();
            }
        }

        #endregion
    }
}