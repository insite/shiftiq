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
    public static class MrProgramRepository
    {
        #region Classes

        private class MrProgramReadHelper : ReadHelper<MrProgram>
        {
            public static readonly MrProgramReadHelper Instance = new MrProgramReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<MrProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.MrPrograms.AsQueryable().AsNoTracking();

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
                db.Entry(new MrProgram { MrProgramId = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static MrProgram Update(int id, Action<MrProgram> action)
        {
            using (var context = new InternalDbContext())
            {
                var entity = context.MrPrograms.Single(x => x.MrProgramId == id);

                action(entity);

                context.SaveChanges();

                return entity;
            }
        }

        #endregion

        #region INSERT

        public static void CopySurveyResponses(int sourceSurveyYear, int destSurveyYear, DateTimeOffset deadline)
        {
            const string query = "EXEC custom_ncsha.CopyMrProgram @SourceSurveyYear, @DestSurveyYear, @Deadline";

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

        public static MrProgram Insert(MrProgram program)
        {
            using (var db = new InternalDbContext())
            {
                db.MrPrograms.Add(program);
                db.SaveChanges();
                return program;
            }
        }

        #endregion

        #region SELECT

        public static int? SelectLastYear()
        {
            using (var db = new InternalDbContext())
                return db.MrPrograms.Max(x => (int?)x.SurveyYear);
        }

        public static IReadOnlyList<MrProgram> Select(
            Expression<Func<MrProgram, bool>> filter,
            params Expression<Func<MrProgram, object>>[] includes) =>
            MrProgramReadHelper.Instance.Select(filter, includes);

        public static MrProgram Select(int id, params Expression<Func<MrProgram, object>>[] includes) =>
            MrProgramReadHelper.Instance.SelectFirst(x => x.MrProgramId == id, includes);

        public static MrProgram SelectFirst(
            Expression<Func<MrProgram, bool>> filter,
            string sortExpression = null,
            params Expression<Func<MrProgram, object>>[] includes) =>
            MrProgramReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static T Bind<T>(int id, Expression<Func<MrProgram, T>> binder) =>
            MrProgramReadHelper.Instance.BindFirst(binder, x => x.MrProgramId == id);

        public static int Count(Expression<Func<MrProgram, bool>> filter) =>
            MrProgramReadHelper.Instance.Count(filter);

        #endregion

        #region SelectByFilter

        public static SearchResultList SelectByFilter(MrProgramFilter filter)
        {
            var sortExpression = "SurveyYear DESC, AgencyName ASC, ID";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ID = x.MrProgramId,
                        x.SurveyYear,
                        AgencyName = x.MR001,
                        RespondentName = x.MR002,
                        x.DateTimeSaved,
                        x.DateTimeSubmitted
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static int CountByFilter(MrProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        private static IQueryable<MrProgram> CreateQueryByFilter(MrProgramFilter filter, InternalDbContext db)
        {
            var query = db.MrPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AgencyName))
                query = query.Where(x => x.MR001.Contains(filter.AgencyName));

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
