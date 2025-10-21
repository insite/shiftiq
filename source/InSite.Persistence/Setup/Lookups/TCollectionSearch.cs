using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TCollectionSearch
    {
        #region Classes

        public class ReferenceInfo
        {
            public string ColumnName { get; set; }
            public string TableName { get; set; }
            public string SchemaName { get; set; }

            public bool IsValid => IsCharType || IsGuidType;
            public bool IsCharType { get; set; }
            public bool IsGuidType { get; set; }
            public bool HasOrganizationColumn { get; set; }
        }

        private class ReferenceProbeResult
        {
            public string ColumnTypeName { get; set; }
            public string OrganizationTypeName { get; set; }
        }

        #endregion

        #region Methods (helpers)

        public static ReferenceInfo[] ParseCollectionReferences(string reference)
        {
            return !reference.HasValue()
                ? null
                : reference.Split(',').Select(x => ParseReferenceInfo(x)).ToArray();
        }

        public static ReferenceInfo ParseReferenceInfo(string reference)
        {
            const string query = @"
SELECT
    (SELECT
        tp.[name]
     FROM
        sys.columns AS c
        INNER JOIN sys.types AS tp ON tp.user_type_id = c.user_type_id
     WHERE
        c.[object_id] = tbl.[object_id]
        AND c.[name] = '{2}'
    ) AS ColumnTypeName
   ,(SELECT
        tp.[name]
     FROM
        sys.columns AS c
        INNER JOIN sys.types AS tp ON tp.user_type_id = c.user_type_id
     WHERE
        c.[object_id] = tbl.[object_id]
        AND c.[name] = 'OrganizationIdentifier'
    ) AS OrganizationTypeName
FROM
    sys.tables AS tbl
WHERE
    tbl.[schema_id] = SCHEMA_ID('{0}')
    AND tbl.[name] = '{1}';";

            if (!reference.HasValue())
                return null;

            var strs = reference.Trim().Split('.');
            if (strs.Length != 3)
                return null;

            var result = new ReferenceInfo
            {
                SchemaName = strs[0],
                TableName = strs[1],
                ColumnName = strs[2]
            };

            using (var db = new InternalDbContext())
            {
                var probe = db.Database
                    .SqlQuery<ReferenceProbeResult>(query.Format(
                        result.SchemaName.Replace("'", "''"),
                        result.TableName.Replace("'", "''"),
                        result.ColumnName.Replace("'", "''")))
                    .FirstOrDefault();

                if (probe != null)
                {
                    result.IsCharType = probe.ColumnTypeName == "varchar" || probe.ColumnTypeName == "nvarchar";
                    result.IsGuidType = probe.ColumnTypeName == "uniqueidentifier";
                    result.HasOrganizationColumn = probe.OrganizationTypeName == "uniqueidentifier";
                }
            }

            return result;
        }

        #endregion

        public static TCollection Select(Guid id) =>
            SearchHelper.Instance.SelectFirst(x => x.CollectionIdentifier == id, null);

        public static T[] Bind<T>(Expression<Func<TCollection, T>> binder, TCollectionFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return Filter(db.TCollections, filter)
                    .Select(binder)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        public static T BindFirst<T>(Expression<Func<TCollection, T>> binder, TCollectionFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return Filter(db.TCollections, filter)
                    .Select(binder)
                    .OrderBy(filter.OrderBy)
                    .FirstOrDefault();
            }
        }

        public static T[] Distinct<T>(
            Expression<Func<TCollection, T>> binder,
            Expression<Func<TCollection, bool>> filter,
            string modelSort = null
            ) =>
            SearchHelper.Instance.Distinct(binder, filter, modelSort);

        public static bool Exists(TCollectionFilter filter) =>
            SearchHelper.Instance.Exists((IQueryable<TCollection> query) => Filter(query, filter));

        public static int Count(TCollectionFilter filter) =>
            SearchHelper.Instance.Count((IQueryable<TCollection> query) => Filter(query, filter));

        #region Helpers

        private class SearchHelper : ReadHelper<TCollection>
        {
            public static readonly SearchHelper Instance = new SearchHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TCollection>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TCollections.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        private static IQueryable<TCollection> Filter(IQueryable<TCollection> query, TCollectionFilter filter)
        {
            if (filter.ExcludeCollectionIdentifier.HasValue)
                query = query.Where(x => x.CollectionIdentifier != filter.ExcludeCollectionIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.CollectionName))
                query = query.Where(x => x.CollectionName.Contains(filter.CollectionName));

            if (!string.IsNullOrEmpty(filter.CollectionTool))
                query = query.Where(x => x.CollectionTool.Contains(filter.CollectionTool));

            if (!string.IsNullOrEmpty(filter.CollectionProcess))
                query = query.Where(x => x.CollectionProcess.Contains(filter.CollectionProcess));

            if (!string.IsNullOrEmpty(filter.CollectionType))
                query = query.Where(x => x.CollectionType.Contains(filter.CollectionType));

            return query;
        }

        #endregion
    }
}
