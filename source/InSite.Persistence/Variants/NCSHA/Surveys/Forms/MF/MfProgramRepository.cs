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
    public static class MfProgramRepository
    {
        #region Classes

        private class MfProgramReadHelper : ReadHelper<MfProgram>
        {
            public static readonly MfProgramReadHelper Instance = new MfProgramReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<MfProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.MfPrograms.AsQueryable().AsNoTracking();

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
                db.Entry(new MfProgram { MfProgramId = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static MfProgram Update(int id, Action<MfProgram> action)
        {
            using (var context = new InternalDbContext())
            {
                var entity = context.MfPrograms.Single(x => x.MfProgramId == id);

                action(entity);

                context.SaveChanges();

                return entity;
            }
        }

        #endregion

        #region INSERT

        public static void CopySurveyResponses(int sourceSurveyYear, int destSurveyYear, DateTimeOffset deadline)
        {
            const string query = "EXEC custom_ncsha.CopyMfProgram @SourceSurveyYear, @DestSurveyYear, @Deadline";

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

        public static MfProgram Insert(MfProgram program)
        {
            using (var db = new InternalDbContext())
            {
                db.MfPrograms.Add(program);
                db.SaveChanges();
                return program;
            }
        }

        #endregion

        #region SELECT

        public static int? SelectLastYear()
        {
            using (var db = new InternalDbContext())
                return db.MfPrograms.Max(x => (int?)x.SurveyYear);
        }

        public static IReadOnlyList<MfProgram> Select(
            Expression<Func<MfProgram, bool>> filter,
            params Expression<Func<MfProgram, object>>[] includes) =>
            MfProgramReadHelper.Instance.Select(filter, includes);

        public static MfProgram Select(int id, params Expression<Func<MfProgram, object>>[] includes) =>
            MfProgramReadHelper.Instance.SelectFirst(x => x.MfProgramId == id, includes);

        public static MfProgram SelectFirst(
            Expression<Func<MfProgram, bool>> filter,
            string sortExpression = null,
            params Expression<Func<MfProgram, object>>[] includes) =>
            MfProgramReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static T Bind<T>(int id, Expression<Func<MfProgram, T>> binder) =>
            MfProgramReadHelper.Instance.BindFirst(binder, x => x.MfProgramId == id);

        public static int Count(Expression<Func<MfProgram, bool>> filter) =>
            MfProgramReadHelper.Instance.Count(filter);

        #endregion

        #region SelectByFilter

        public static SearchResultList SelectByFilter(MfProgramFilter filter)
        {
            var sortExpression = "SurveyYear DESC, AgencyName ASC, ID";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ID = x.MfProgramId,
                        x.SurveyYear,
                        AgencyName = x.MF001,
                        RespondentName = x.MF002,
                        x.DateTimeSaved,
                        x.DateTimeSubmitted
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static int CountByFilter(MfProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        private static IQueryable<MfProgram> CreateQueryByFilter(MfProgramFilter filter, InternalDbContext db)
        {
            var query = db.MfPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AgencyName))
                query = query.Where(x => x.MF001.Contains(filter.AgencyName));

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
