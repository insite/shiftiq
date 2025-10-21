using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Shift.Common;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class CounterRepository
    {
        #region Classes

        private class CounterReadHelper : ReadHelper<Counter>
        {
            public static readonly CounterReadHelper Instance = new CounterReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Counter>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Counters.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region Fields

        private static readonly object _refreshSyncRoot = new object();

        #endregion

        #region Read

        public static IReadOnlyList<Counter> Select(
            Expression<Func<Counter, bool>> filter,
            params Expression<Func<Counter, object>>[] includes)
        {
            return CounterReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<Counter> Select(
            Expression<Func<Counter, bool>> filter,
            string sortExpression,
            params Expression<Func<Counter, object>>[] includes)
        {
            return CounterReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<Counter, T>> binder,
            Expression<Func<Counter, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return CounterReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<Counter, T>> binder,
            Expression<Func<Counter, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return CounterReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static T[] Distinct<T>(
            Expression<Func<Counter, T>> binder,
            Expression<Func<Counter, bool>> filter = null,
            string modelSort = null)
        {
            return CounterReadHelper.Instance.Distinct(binder, filter, modelSort);
        }

        public static int Count(Expression<Func<Counter, bool>> filter) =>
            CounterReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<Counter, bool>> filter) =>
            CounterReadHelper.Instance.Exists(filter);


        #endregion

        #region Write

        public static int Refresh(IEnumerable<int> years, IEnumerable<Field> fields)
        {
            var refreshCte = BuildRefreshCteQuery(fields);
            if (refreshCte == null)
            {
                return 0;
            }

            var where = BuildWhere();

            using (var context = new InternalDbContext())
            {
                var testQuery = refreshCte + " SELECT TOP 1 1 FROM CTE " + where + ";";
                if (context.Database.SqlQuery<int?>(testQuery).FirstOrDefault() != 1)
                {
                    return 0;
                }

                var refreshQuery = $@"
DELETE FROM custom_ncsha.Counter WHERE Scope <> '*National Weighted Average';

{refreshCte}
INSERT INTO custom_ncsha.[Counter] (
    [{nameof(Counter.Category)}]
   ,[{nameof(Counter.Code)}]
   ,[{nameof(Counter.Name)}]
   ,[{nameof(Counter.Scope)}]
   ,[{nameof(Counter.Year)}]
   ,[{nameof(Counter.Value)}]
   ,[{nameof(Counter.Unit)}]
)
SELECT
    Category
   ,Code
   ,[Name]
   ,Scope
   ,[Year]
   ,CAST(REPLACE([Value],',','') AS decimal(16,2)) AS [Value]
   ,[Unit]
FROM
    CTE 
{where}
AND NOT EXISTS (SELECT TOP 1 * FROM custom_ncsha.[Counter] WHERE [Counter].Code = CTE.Code AND [Counter].Scope = CTE.Scope AND [Counter].[Year] = CTE.[Year])
;

SELECT @@ROWCOUNT;";

                var postRefreshQuery = $@"
DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC019' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC213", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC019';


DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC020' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC214", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC020';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC021' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC215", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC021';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC023' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC216", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC023';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC024' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC217", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC024';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC025' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC218", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC025';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC027' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC219", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC027';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC028' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC220", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC028';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC029' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC221", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC029';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC031' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC222", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC031';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC032' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC223", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC032';



DELETE FROM custom_ncsha.[Counter] WHERE Code = 'HC033' AND [Year] <= 2008;

{BuildRefreshCteQuery(new Field[] { new Field { Code = "HC224", IsNumeric = true, Name = "None", Category = "None", Unit = "None", } })}
UPDATE
    custom_ncsha.[Counter]
SET
    [Value] = [Counter].[Value] - CAST(REPLACE(CTE.[Value],',','') AS decimal(16,2))
FROM
    custom_ncsha.[Counter]
    INNER JOIN CTE ON CTE.[Year] = [Counter].[Year] AND CTE.Scope = [Counter].Scope
{where}
    AND [Counter].Code = 'HC033';



UPDATE
    custom_ncsha.[Counter]
SET
    [Name] = ValueHelper.[Name]
FROM
    custom_ncsha.[Counter] AS q1
    OUTER APPLY (
        SELECT
            q2.[Name]
        FROM
            custom_ncsha.[Counter] AS q2
        WHERE
            q1.Code = q2.Code
            AND q1.Scope <> q2.Scope
    ) AS ValueHelper
WHERE
    Scope = '*National Weighted Average'
    AND ValueHelper.[Name] IS NOT NULL
    AND q1.[Name] <> ValueHelper.[Name];
";

                lock (_refreshSyncRoot)
                {
                    context.Database.CommandTimeout = 240;

                    var updated = context.Database.SqlQuery<int>(refreshQuery).Single();

                    context.Database.ExecuteSqlCommand(postRefreshQuery);

                    return updated;
                }
            }

            string BuildWhere()
            {
                var sb = new StringBuilder(
                      "WHERE"
                    + " CTE.[Value] IS NOT NULL"
                    + " AND CTE.[Value] NOT IN ('N/AV', 'N/AP')"
                    + " AND CTE.Scope IS NOT NULL"
                    + " AND CTE.RowNumber = 1"
                    + " AND ISNUMERIC(REPLACE(CTE.[Value],',','')) = 1");

                if (years != null && years.Any())
                {
                    sb.Append(" AND CTE.[Year] IN (").Append(string.Join(",", years)).Append(")");
                }

                return sb.ToString();
            }
        }

        private static string BuildRefreshCteQuery(IEnumerable<Field> fields)
        {
            var query = new StringBuilder();

            query.AppendLine("WITH CTE (Category,Code,[Name],Scope,[Year],[Value],Unit,RowNumber) AS (");

            var isFirstField = true;

            foreach (var field in fields)
            {
                if (!field.IsNumeric)
                {
                    continue;
                }

                if (isFirstField)
                {
                    isFirstField = false;
                }
                else
                {
                    query.Append("UNION ");
                }

                if (field.Code.Length < 5)
                {
                    throw new ApplicationError("Invalid field code: " + field.Code);
                }

                var tableCode = field.Code.Substring(0, 2);

                query
                    .Append("SELECT");
                query
                    .AppendFormat(" '{0}'", field.Category.Replace("'", "''"))
                    .AppendFormat(",'{0}'", field.Code.Replace("'", "''"))
                    .AppendFormat(",'{0}'", field.Name.Replace("'", "''"))
                    .Append(",ISNULL(StateName, '')")
                    .Append(",SurveyYear")
                    .AppendFormat(",{0}", field.Code)
                    .AppendFormat(",'{0}'", field.Unit.Replace("'", "''"))
                    .AppendFormat(",ROW_NUMBER() OVER(PARTITION BY ISNULL(StateName, ''),SurveyYear ORDER BY ISNULL(DateTimeSubmitted,ISNULL(DateTimeSaved,InsertedOn)) DESC, {0}ProgramID DESC) AS RowNumber", tableCode)
                    ;
                query
                    .AppendFormat(" FROM [custom_ncsha].[{0}Program]", tableCode);

                query.AppendLine();
            }

            query
                .AppendLine(")")
                .AppendLine();

            return isFirstField ? null : query.ToString();
        }

        #endregion
    }
}
