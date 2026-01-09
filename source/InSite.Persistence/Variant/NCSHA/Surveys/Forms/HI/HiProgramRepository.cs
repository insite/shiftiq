using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class HiProgramRepository
    {
        #region Classes

        private class HiProgramReadHelper : ReadHelper<HiProgram>
        {
            public static readonly HiProgramReadHelper Instance = new HiProgramReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<HiProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.HiPrograms.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region DELETE

        public static void Delete(int id)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(new HiProgram { HiProgramId = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static HiProgram Update(int id, Action<HiProgram> action)
        {
            using (var context = new InternalDbContext())
            {
                var entity = context.HiPrograms.Single(x => x.HiProgramId == id);

                action(entity);

                context.SaveChanges();

                return entity;
            }
        }

        #endregion

        #region INSERT

        public static void CopySurveyResponses(int sourceSurveyYear, int destSurveyYear, DateTimeOffset deadline)
        {
            const string query = "EXEC custom_ncsha.CopyHiProgram @SourceSurveyYear, @DestSurveyYear, @Deadline";

            using (var db = new InternalDbContext())
            {
                var parameters = new[]
                {
                    new SqlParameter("@SourceSurveyYear", sourceSurveyYear),
                    new SqlParameter("@DestSurveyYear", destSurveyYear),
                    new SqlParameter("@Deadline", deadline)
                };

                db.Database.ExecuteSqlCommand(query, parameters);
            }
        }

        public static HiProgram Insert(HiProgram program)
        {
            using (var db = new InternalDbContext())
            {
                db.HiPrograms.Add(program);
                db.SaveChanges();
                return program;
            }
        }

        #endregion

        #region SELECT

        public static int? SelectLastYear()
        {
            using (var db = new InternalDbContext())
                return db.HiPrograms.Max(x => (int?)x.SurveyYear);
        }

        public static IReadOnlyList<HiProgram> Select(
            Expression<Func<HiProgram, bool>> filter,
            params Expression<Func<HiProgram, object>>[] includes) =>
            HiProgramReadHelper.Instance.Select(filter, includes);

        public static HiProgram Select(int id, params Expression<Func<HiProgram, object>>[] includes) =>
            HiProgramReadHelper.Instance.SelectFirst(x => x.HiProgramId == id, includes);

        public static HiProgram SelectFirst(
            Expression<Func<HiProgram, bool>> filter,
            string sortExpression = null,
            params Expression<Func<HiProgram, object>>[] includes) =>
            HiProgramReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static T Bind<T>(int id, Expression<Func<HiProgram, T>> binder) =>
            HiProgramReadHelper.Instance.BindFirst(binder, x => x.HiProgramId == id);

        public static int Count(Expression<Func<HiProgram, bool>> filter) =>
            HiProgramReadHelper.Instance.Count(filter);

        #endregion

        #region SelectByFilter

        public static SearchResultList SelectByFilter(HiProgramFilter filter)
        {
            var sortExpression = "SurveyYear DESC, AgencyName ASC, ID";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ID = x.HiProgramId,
                        x.SurveyYear,
                        AgencyName = x.HI001,
                        RespondentName = x.HI002,
                        x.DateTimeSaved,
                        x.DateTimeSubmitted
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static int CountByFilter(HiProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        private static IQueryable<HiProgram> CreateQueryByFilter(HiProgramFilter filter, InternalDbContext db)
        {
            var query = db.HiPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AgencyName))
                query = query.Where(x => x.HI001.Contains(filter.AgencyName));

            if (filter.SurveyYear.HasValue)
                query = query.Where(x => x.SurveyYear == filter.SurveyYear);

            if (filter.DateTimeSavedSince.HasValue)
                query = query.Where(x => x.DateTimeSaved >= filter.DateTimeSavedSince.Value);

            if (filter.DateTimeSavedBefore.HasValue)
                query = query.Where(x => x.DateTimeSaved < filter.DateTimeSavedBefore.Value);

            return query;
        }

        #endregion
    }
}
