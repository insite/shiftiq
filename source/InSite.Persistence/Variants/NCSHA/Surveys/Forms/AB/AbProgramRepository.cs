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
    public static class AbProgramRepository
    {
        #region Classes

        private class AbProgramReadHelper : ReadHelper<AbProgram>
        {
            public static readonly AbProgramReadHelper Instance = new AbProgramReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<AbProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.AbPrograms.AsQueryable().AsNoTracking();

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
                db.Entry(new AbProgram { AbProgramId = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static AbProgram Update(int id, Action<AbProgram> action)
        {
            using (var context = new InternalDbContext())
            {
                var entity = context.AbPrograms.Single(x => x.AbProgramId == id);

                action(entity);

                context.SaveChanges();

                return entity;
            }
        }

        #endregion

        #region INSERT

        public static void CopySurveyResponses(int sourceSurveyYear, int destSurveyYear, DateTimeOffset deadline)
        {
            const string query = "EXEC custom_ncsha.CopyAbProgram @SourceSurveyYear, @DestSurveyYear, @Deadline";

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

        public static AbProgram Insert(AbProgram program)
        {
            using (var db = new InternalDbContext())
            {
                db.AbPrograms.Add(program);
                db.SaveChanges();
                return program;
            }
        }

        #endregion

        #region SELECT

        public static int? SelectLastYear()
        {
            using (var db = new InternalDbContext())
                return db.AbPrograms.Max(x => (int?)x.SurveyYear);
        }

        public static IReadOnlyList<AbProgram> Select(
            Expression<Func<AbProgram, bool>> filter,
            params Expression<Func<AbProgram, object>>[] includes) =>
            AbProgramReadHelper.Instance.Select(filter, includes);

        public static AbProgram Select(int id, params Expression<Func<AbProgram, object>>[] includes) =>
            AbProgramReadHelper.Instance.SelectFirst(x => x.AbProgramId == id, includes);

        public static AbProgram SelectFirst(
            Expression<Func<AbProgram, bool>> filter,
            string sortExpression = null,
            params Expression<Func<AbProgram, object>>[] includes) =>
            AbProgramReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static T Bind<T>(int id, Expression<Func<AbProgram, T>> binder) =>
            AbProgramReadHelper.Instance.BindFirst(binder, x => x.AbProgramId == id);

        public static int Count(Expression<Func<AbProgram, bool>> filter) =>
            AbProgramReadHelper.Instance.Count(filter);

        #endregion

        #region SelectByFilter

        public static SearchResultList SelectByFilter(AbProgramFilter filter)
        {
            var sortExpression = "SurveyYear DESC, AgencyName ASC, ID";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ID = x.AbProgramId,
                        x.SurveyYear,
                        AgencyName = x.AB001,
                        RespondentName = x.AB002,
                        x.DateTimeSaved,
                        x.DateTimeSubmitted
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static int CountByFilter(AbProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        private static IQueryable<AbProgram> CreateQueryByFilter(AbProgramFilter filter, InternalDbContext db)
        {
            var query = db.AbPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AgencyName))
                query = query.Where(x => x.AB001.Contains(filter.AgencyName));

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