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
    public static class PaProgramRepository
    {
        #region Classes

        private class PaProgramReadHelper : ReadHelper<PaProgram>
        {
            public static readonly PaProgramReadHelper Instance = new PaProgramReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<PaProgram>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.PaPrograms.AsQueryable().AsNoTracking();

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
                db.Entry(new PaProgram { PaProgramId = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region UPDATE

        public static PaProgram Update(int id, Action<PaProgram> action)
        {
            using (var context = new InternalDbContext())
            {
                var entity = context.PaPrograms.Single(x => x.PaProgramId == id);

                action(entity);

                context.SaveChanges();

                return entity;
            }
        }

        #endregion

        #region INSERT

        public static void CopySurveyResponses(int sourceSurveyYear, int destSurveyYear, DateTimeOffset deadline)
        {
            const string query = "EXEC custom_ncsha.CopyPaProgram @SourceSurveyYear, @DestSurveyYear, @Deadline";

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

        public static PaProgram Insert(PaProgram program)
        {
            using (var db = new InternalDbContext())
            {
                db.PaPrograms.Add(program);
                db.SaveChanges();
                return program;
            }
        }

        #endregion

        #region SELECT

        public static int? SelectLastYear()
        {
            using (var db = new InternalDbContext())
                return db.PaPrograms.Max(x => (int?)x.SurveyYear);
        }

        public static IReadOnlyList<PaProgram> Select(
            Expression<Func<PaProgram, bool>> filter,
            params Expression<Func<PaProgram, object>>[] includes) =>
            PaProgramReadHelper.Instance.Select(filter, includes);

        public static PaProgram Select(int id, params Expression<Func<PaProgram, object>>[] includes) =>
            PaProgramReadHelper.Instance.SelectFirst(x => x.PaProgramId == id, includes);

        public static PaProgram SelectFirst(
            Expression<Func<PaProgram, bool>> filter,
            string sortExpression = null,
            params Expression<Func<PaProgram, object>>[] includes) =>
            PaProgramReadHelper.Instance.SelectFirst(filter, sortExpression, includes);

        public static T Bind<T>(int id, Expression<Func<PaProgram, T>> binder) =>
            PaProgramReadHelper.Instance.BindFirst(binder, x => x.PaProgramId == id);

        public static int Count(Expression<Func<PaProgram, bool>> filter) =>
            PaProgramReadHelper.Instance.Count(filter);

        #endregion

        #region SelectByFilter

        public static SearchResultList SelectByFilter(PaProgramFilter filter)
        {
            var sortExpression = "SurveyYear DESC, AgencyName ASC, ID";

            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .Select(x => new
                    {
                        ID = x.PaProgramId,
                        x.SurveyYear,
                        AgencyName = x.PA001,
                        RespondentName = x.PA002,
                        x.DateTimeSaved,
                        x.DateTimeSubmitted
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        public static int CountByFilter(PaProgramFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        private static IQueryable<PaProgram> CreateQueryByFilter(PaProgramFilter filter, InternalDbContext db)
        {
            var query = db.PaPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.AgencyName))
                query = query.Where(x => x.PA001.Contains(filter.AgencyName));

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
