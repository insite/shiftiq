using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Shift.Common.Linq;

using Shift.Constant;

namespace InSite.Persistence
{
    public class VForeignKeySearch
    {
        public static int Count(VForeignKeyFilter filter)
        {
            using (var db = new ReportDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static IListSource Select(VForeignKeyFilter filter)
        {
            var sortExpression = "ForeignSchemaName, ForeignTableName, ForeignColumnName";

            using (var db = new ReportDbContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToSearchResult();
            }
        }

        private static IQueryable<VForeignKey> CreateQuery(VForeignKeyFilter filter, ReportDbContext db)
        {
            var query = db.VForeignKeys.AsQueryable();

            if (!string.IsNullOrEmpty(filter.SchemaName))
            {
                if (filter.IsExactComparison)
                    query = query.Where(x => x.ForeignSchemaName == filter.SchemaName);
                else
                    query = query.Where(x => x.ForeignSchemaName.Contains(filter.SchemaName));
            }

            if (!string.IsNullOrEmpty(filter.TableName))
            {
                if (filter.IsExactComparison)
                    query = query.Where(x => x.ForeignTableName == filter.TableName);
                else
                    query = query.Where(x => x.ForeignTableName.Contains(filter.TableName));
            }

            if (!string.IsNullOrEmpty(filter.ColumnName))
            {
                if (filter.IsExactComparison)
                    query = query.Where(x => x.ForeignColumnName == filter.ColumnName);
                else
                    query = query.Where(x => x.ForeignColumnName.Contains(filter.ColumnName));
            }

            if (!string.IsNullOrEmpty(filter.PrimarySchemaName))
            {
                if (filter.IsExactComparison)
                    query = query.Where(x => x.PrimarySchemaName == filter.PrimarySchemaName);
                else
                    query = query.Where(x => x.PrimarySchemaName.Contains(filter.PrimarySchemaName));
            }

            if (!string.IsNullOrEmpty(filter.PrimaryTableName))
            {
                if (filter.IsExactComparison)
                    query = query.Where(x => x.PrimaryTableName == filter.PrimaryTableName);
                else
                    query = query.Where(x => x.PrimaryTableName.Contains(filter.PrimaryTableName));
            }

            if (filter.EnforcedInclusion.HasValue)
            {
                if (filter.EnforcedInclusion == InclusionType.Exclude)
                    query = query.Where(x => !x.IsEnforced);
                else if (filter.EnforcedInclusion == InclusionType.Only)
                    query = query.Where(x => x.IsEnforced);
            }

            if (!string.IsNullOrEmpty(filter.UniqueName))
                query = query.Where(x => x.UniqueName == filter.UniqueName);

            return query;
        }

        public static List<T> Select<T>(string query)
        {
            using (var db = new ReportDbContext())
            {
                return db.Database.SqlQuery<T>(query).ToList();
            }
        }
    }
}