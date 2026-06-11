using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class VAchievementCategorySearch
    {
        private class VAchievementCategoryReadHelper : ReadHelper<VAchievementCategory>
        {
            public static readonly VAchievementCategoryReadHelper Instance = new VAchievementCategoryReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VAchievementCategory>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    context.Configuration.LazyLoadingEnabled = false;

                    var query = context.VAchievementCategories.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static VAchievementCategory SelectFirst(Expression<Func<VAchievementCategory, bool>> filter,
            params Expression<Func<VAchievementCategory, object>>[] includes)
        {
            return VAchievementCategoryReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<VAchievementCategory> Select(
            Expression<Func<VAchievementCategory, bool>> filter,
            params Expression<Func<VAchievementCategory, object>>[] includes)
        {
            return VAchievementCategoryReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<VAchievementCategory> Select(
            Expression<Func<VAchievementCategory, bool>> filter,
            string sortExpression,
            params Expression<Func<VAchievementCategory, object>>[] includes)
        {
            return VAchievementCategoryReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static T[] Bind<T>(
            Expression<Func<VAchievementCategory, T>> binder,
            Expression<Func<VAchievementCategory, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return VAchievementCategoryReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VAchievementCategory, T>> binder,
            Expression<Func<VAchievementCategory, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return VAchievementCategoryReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static int Count(Expression<Func<VAchievementCategory, bool>> filter)
        {
            return VAchievementCategoryReadHelper.Instance.Count(filter);
        }

        public static bool Exists(Expression<Func<VAchievementCategory, bool>> filter)
        {
            return VAchievementCategoryReadHelper.Instance.Exists(filter);
        }

        public static VAchievementCategory Select(Guid categoryIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.VAchievementCategories.FirstOrDefault(x => x.CategoryIdentifier == categoryIdentifier);
        }

        public static VAchievementCategory Select(Guid achievement, Guid organizationIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.VAchievementClassifications
                    .Where(x => x.AchievementIdentifier == achievement && x.Category.OrganizationIdentifier == organizationIdentifier)
                    .Select(x => x.Category)
                    .FirstOrDefault();
            }
        }

        public static DataTable SelectForSelector(VAchievementCategoryFilter filter)
        {
            DataTable table;

            using (var db = new InternalDbContext())
            {
                table = CreateQuery(filter, db)
                    .Select(x => new
                    {
                        Value = x.CategoryIdentifier,
                        AchievementLabel = x.AchievementLabel ?? "?",
                        CategoryName = x.CategoryName
                    })
                    .OrderBy("AchievementLabel, CategoryName")
                    .ToDataTable();
            }

            table.Columns.Add("Text");

            foreach (DataRow row in table.Rows)
                row["Text"] = StringHelper.Acronym(AchievementTypes.Pluralize((string)row["AchievementLabel"], filter.OrganizationCode)) + ": " + (string)row["CategoryName"];

            return table;
        }

        public static DataTable SelectByFilter(VAchievementCategoryFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Join(db.Organizations,
                        a => a.OrganizationIdentifier,
                        b => b.OrganizationIdentifier,
                        (a, b) => new
                        {
                            CompanyName = b.CompanyTitle,
                            AchievementLabel = a.AchievementLabel,
                            CategoryName = a.CategoryName,
                            CategoryIdentifier = a.CategoryIdentifier
                        }
                    )
                    .OrderBy(x => x.CompanyName)
                    .ThenBy(x => x.AchievementLabel)
                    .ThenBy(x => x.CategoryName)
                    .ApplyPaging(filter)
                    .ToDataTable();
            }
        }

        public static int CountByFilter(VAchievementCategoryFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        private static IQueryable<VAchievementCategory> CreateQuery(VAchievementCategoryFilter filter, InternalDbContext db)
        {
            var query = db.VAchievementCategories.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.CategoryName))
                query = query.Where(x => x.CategoryName.Contains(filter.CategoryName));

            if (!string.IsNullOrEmpty(filter.AchievementLabel))
                query = query.Where(x => x.AchievementLabel == filter.AchievementLabel);

            return query;
        }
    }
}