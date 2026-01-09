using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Humanizer;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class TEntitySearch
    {
        public static string ConnectionString => new ReportDbContext().Database.Connection.ConnectionString;

        public static int Count(TEntityFilter filter)
        {
            using (var context = new ReportDbContext())
            {
                return CreateQuery(context, filter).Count();
            }
        }

        public static List<TEntity> GetOrphanEntities()
        {
            var query = @"
SELECT E.*
FROM metadata.TEntity AS E
WHERE NOT EXISTS (SELECT * FROM databases.VTable AS T WHERE T.SchemaName = E.StorageSchema AND T.TableName = E.StorageTable)
ORDER BY e.StorageStructure, E.StorageSchema, E.StorageTable, E.ComponentType, E.ComponentName, E.ComponentPart, E.EntityName
                ";

            using (var db = new ReportDbContext())
            {
                return db.Database.SqlQuery<TEntity>(query).ToList();
            }
        }

        public static List<StorageObject> GetOrphanTables()
        {
            var query = @"
SELECT T.SchemaName, T.TableName, 'Table' as TableType, T.ColumnCount, T.[RowCount], CAST(T.DateCreated AS DATE) AS Created
FROM databases.VTable AS T 
WHERE NOT EXISTS (SELECT * FROM metadata.TEntity AS E WHERE T.SchemaName = E.StorageSchema AND T.TableName = E.StorageTable) 
  AND T.SchemaName NOT IN ('backups','dbo')
  AND T.SchemaName + '.' + T.TableName NOT IN ('metadata.TEntity')
  AND T.TableName NOT LIKE 'B%'
  AND T.TableName NOT LIKE '%Buffer'
ORDER BY T.SchemaName, T.TableName
                ";

            using (var db = new ReportDbContext())
            {
                return db.Database.SqlQuery<StorageObject>(query).ToList();
            }
        }

        public static List<UnexpectedCollectionItem> GetUnexpectedCollections()
        {
            var query = @"
SELECT E.*
FROM metadata.TEntity AS E
ORDER BY E.EntityName
                ";

            var list = new List<UnexpectedCollectionItem>();

            using (var db = new ReportDbContext())
            {
                var entities = db.Database.SqlQuery<TEntity>(query).ToList();

                foreach (var entity in entities)
                {
                    var segments = entity.EntityName.ToKebabCase().Split('-');

                    var names = new List<string>();

                    foreach (var singular in segments)
                    {
                        var isExternalPlatform = entity.ComponentName == "Integration" && singular == segments.First();

                        var isNcshaSurveyTable = entity.ComponentPart == "NCSHA" && StringHelper.EqualsAny(singular, new[] { "ab", "hc", "hi", "mf", "mr", "pa" });

                        if (isExternalPlatform || isNcshaSurveyTable)
                        {
                            names.Add(singular);
                        }
                        else
                        {
                            names.Add(singular.Pluralize());
                        }
                    }

                    var kebab = string.Join("-", names);

                    if (kebab != entity.CollectionSlug)
                    {
                        list.Add(new UnexpectedCollectionItem
                        {
                            EntityName = entity.EntityName,
                            ActualCollectionSlug = entity.CollectionSlug,
                            ExpectedCollectionSlug = kebab
                        });
                    }
                }
            }

            return list;
        }

        public static List<UnexpectedEntityItem> GetUnexpectedEntities()
        {
            var query = @"
SELECT E.*
FROM metadata.TEntity AS E
ORDER BY E.EntityName
                ";

            var list = new List<UnexpectedEntityItem>();

            using (var db = new ReportDbContext())
            {
                var entities = db.Database.SqlQuery<TEntity>(query).ToList();

                foreach (var entity in entities)
                {
                    var expectedEntityName = (entity.StorageTableRename ?? entity.StorageTable).Substring(1);

                    if (expectedEntityName != entity.EntityName)
                    {
                        list.Add(new UnexpectedEntityItem
                        {
                            StorageTable = entity.StorageTableRename ?? entity.StorageTable,
                            ActualEntityName = entity.EntityName,
                            ExpectedEntityName = expectedEntityName
                        });
                    }
                }
            }

            return list;
        }

        public static List<TEntity> Select(TEntityFilter filter)
        {
            using (var context = new ReportDbContext())
            {
                return CreateQuery(context, filter)
                    .OrderBy(x => x.ComponentType)
                    .ThenBy(x => x.ComponentName)
                    .ThenBy(x => x.ComponentPart)
                    .ThenBy(x => x.EntityName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        #region Methods (helper)

        private static IQueryable<TEntity> CreateQuery(ReportDbContext db, TEntityFilter filter)
        {
            var query = db.TEntities.AsNoTracking().AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.CollectionSlug))
                    query = query.Where(x => x.CollectionSlug.Contains(filter.CollectionSlug));

                if (!string.IsNullOrEmpty(filter.CollectionKey))
                    query = query.Where(x => x.CollectionKey.Contains(filter.CollectionKey));

                if (!string.IsNullOrEmpty(filter.ComponentPart))
                    query = query.Where(x => x.ComponentPart.Contains(filter.ComponentPart));

                if (!string.IsNullOrEmpty(filter.ComponentType))
                    query = query.Where(x => x.ComponentType.Contains(filter.ComponentType));

                if (!string.IsNullOrEmpty(filter.ComponentName))
                    query = query.Where(x => x.ComponentName.Contains(filter.ComponentName));

                if (!string.IsNullOrEmpty(filter.EntityName))
                    query = query.Where(x => x.EntityName.Contains(filter.EntityName));

                if (!string.IsNullOrEmpty(filter.StorageKey))
                    query = query.Where(x => x.StorageKey.Contains(filter.StorageKey));

                if (!string.IsNullOrEmpty(filter.StorageSchema))
                    query = query.Where(x => x.StorageSchema.Contains(filter.StorageSchema));

                if (!string.IsNullOrEmpty(filter.StorageStructure))
                    query = query.Where(x => x.StorageStructure.Contains(filter.StorageStructure));

                if (!string.IsNullOrEmpty(filter.StorageTable))
                    query = query.Where(x => x.StorageTable.Contains(filter.StorageTable));

                if (!string.IsNullOrEmpty(filter.Keyword))
                    query = query.Where(x => x.CollectionSlug.Contains(filter.Keyword)
                                          || x.CollectionKey.Contains(filter.Keyword)
                                          || x.ComponentPart.Contains(filter.Keyword)
                                          || x.ComponentType.Contains(filter.Keyword)
                                          || x.ComponentName.Contains(filter.Keyword)
                                          || x.EntityName.Contains(filter.Keyword)
                                          || x.StorageKey.Contains(filter.Keyword)
                                          || x.StorageSchema.Contains(filter.Keyword)
                                          || x.StorageStructure.Contains(filter.Keyword)
                                          || x.StorageTable.Contains(filter.Keyword)
                                          );
            }

            return query.AsNoTracking();
        }

        public static List<T> Select<T>(string query)
        {
            using (var db = new ReportDbContext())
            {
                return db.Database.SqlQuery<T>(query).ToList();
            }
        }

        #endregion
    }
}