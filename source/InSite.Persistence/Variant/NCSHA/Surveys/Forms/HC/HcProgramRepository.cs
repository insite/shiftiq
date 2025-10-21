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
    public static class HcProgramRepository
    {
        #region Classes

        private class HcProgramReadHelper : ReadHelper<HcProgram>
        {
            public static readonly HcProgramReadHelper Instance = new HcProgramReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<HcProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.HcPrograms.AsQueryable().AsNoTracking();

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
                db.Entry(new HcProgram { HcProgramId = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static HcProgram Update(int id, Action<HcProgram> action)
        {
            using (var context = new InternalDbContext())
            {
                var entity = context.HcPrograms.Single(x => x.HcProgramId == id);

                action(entity);

                context.SaveChanges();

                return entity;
            }
        }

        #endregion

        #region INSERT

        public static void CopySurveyResponses(int sourceSurveyYear, int destSurveyYear, DateTimeOffset deadline)
        {
            const string query = "EXEC custom_ncsha.CopyHcProgram @SourceSurveyYear, @DestSurveyYear, @Deadline";

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

        public static HcProgram Insert(HcProgram program)
        {
            using (var db = new InternalDbContext())
            {
                db.HcPrograms.Add(program);
                db.SaveChanges();
                return program;
            }
        }

        #endregion

        #region SELECT

        public static int? SelectLastYear()
        {
            using (var db = new InternalDbContext())
                return db.HcPrograms.Max(x => (int?)x.SurveyYear);
        }

        public static IReadOnlyList<HcProgram> Select(
            Expression<Func<HcProgram, bool>> filter,
            params Expression<Func<HcProgram, object>>[] includes) =>
            HcProgramReadHelper.Instance.Select(filter, includes);

        public static HcProgram Select(
            int id,
            params Expression<Func<HcProgram, object>>[] includes) =>
            HcProgramReadHelper.Instance.SelectFirst(x => x.HcProgramId == id, includes);

        public static HcProgram SelectFirst(
            Expression<Func<HcProgram, bool>> filter,
            string sortExpression = null,
            params Expression<Func<HcProgram, object>>[] includes) =>
            HcProgramReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static T Bind<T>(
            int id,
            Expression<Func<HcProgram, T>> binder) =>
            HcProgramReadHelper.Instance.BindFirst(binder, x => x.HcProgramId == id);

        public static int Count(Expression<Func<HcProgram, bool>> filter) =>
            HcProgramReadHelper.Instance.Count(filter);

        #endregion

        #region SelectByFilter

        public static SearchResultList SelectByFilter(HcProgramFilter filter)
        {
            var sortExpression = "SurveyYear DESC, AgencyName ASC, ID";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ID = x.HcProgramId,
                        x.SurveyYear,
                        AgencyName = x.HC001,
                        RespondentName = x.HC002,
                        x.DateTimeSaved,
                        x.DateTimeSubmitted
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static int CountByFilter(HcProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        private static IQueryable<HcProgram> CreateQueryByFilter(HcProgramFilter filter, InternalDbContext db)
        {
            var query = db.HcPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AgencyName))
                query = query.Where(x => x.HC001.Contains(filter.AgencyName));

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
