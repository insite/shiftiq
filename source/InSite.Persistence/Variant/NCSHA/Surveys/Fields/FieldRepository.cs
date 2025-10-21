using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Shift.Common;

namespace InSite.Persistence.Plugin.NCSHA
{
    public static class FieldRepository
    {
        #region Classes

        private class FieldReadHelper : ReadHelper<Field>
        {
            public static readonly FieldReadHelper Instance = new FieldReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Field>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Fields.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public class FieldDataEntity
        {
            public int Id { get; set; }
            public int Year { get; set; }
            public string State { get; set; }
            public string Value { get; set; }
        }

        public class FieldMetadataEntity
        {
            public short MaxLength { get; set; }
            public bool IsNullable { get; set; }
        }

        public class MeasurementData
        {
            public string ListFile { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string TagRegion { get; set; }
            public int AsAtYear { get; set; }
            public decimal Quantity { get; set; }
            public string QuantityUnit { get; set; }
        }

        #endregion

        #region Read

        public static Field Select(string code, params Expression<Func<Field, object>>[] includes) =>
            FieldReadHelper.Instance.SelectFirst(x => x.Code == code, includes);

        public static T[] Bind<T>(
            Expression<Func<Field, T>> binder,
            Expression<Func<Field, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return FieldReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T[] Distinct<T>(
            Expression<Func<Field, T>> binder,
            Expression<Func<Field, bool>> filter = null,
            string modelSort = null)
        {
            return FieldReadHelper.Instance.Distinct(binder, filter, modelSort);
        }

        public static int Count(Expression<Func<Field, bool>> filter) =>
            FieldReadHelper.Instance.Count(filter);

        public static int Count(FieldFilter filter) =>
            FieldReadHelper.Instance.Count((IQueryable<Field> query) => Filter(query, filter));

        public static MeasurementData[] GetMeasurementData(IEnumerable<int> years, IEnumerable<Field> fields)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.AppendLine("WITH CTE (ListFile,ItemCode,ItemName,TagRegion,AsAtYear,Quantity,QuantityUnit,RowNumber) AS (");

            var isFirstField = true;

            foreach (var field in fields)
            {
                if (!field.IsNumeric)
                    continue;

                if (isFirstField)
                    isFirstField = false;
                else
                    queryBuilder.Append("UNION ");

                if (field.Code.Length < 5)
                    throw new ApplicationError("Invalid field code: " + field.Code);

                var tableCode = field.Code.Substring(0, 2);

                queryBuilder
                    .Append("SELECT")
                    .AppendFormat(" '{0}'", field.Category.Replace("'", "''"))
                    .AppendFormat(",'{0}'", field.Code.Replace("'", "''"))
                    .AppendFormat(",'{0}'", field.Name.Replace("'", "''"))
                    .Append(",StateName")
                    .Append(",SurveyYear")
                    .AppendFormat(",{0}", field.Code)
                    .AppendFormat(",'{0}'", field.Unit.Replace("'", "''"))
                    .AppendFormat(",ROW_NUMBER() OVER(PARTITION BY StateName,SurveyYear ORDER BY ISNULL(DateTimeSubmitted,ISNULL(DateTimeSaved,InsertedOn)) DESC, {0}ProgramID DESC) AS RowNumber", tableCode)
                    ;
                queryBuilder
                    .AppendFormat(" FROM [custom_ncsha].[{0}Program]", tableCode);

                queryBuilder.AppendLine();
            }

            queryBuilder.Append(@")
SELECT
    ListFile
   ,ItemCode
   ,ItemName
   ,TagRegion
   ,AsAtYear
   ,CAST(REPLACE(Quantity,',','') AS decimal(16,2)) AS Quantity
   ,QuantityUnit
FROM
    CTE 
WHERE
    CTE.TagRegion IS NOT NULL
    AND CTE.RowNumber = 1
    AND CTE.Quantity IS NOT NULL
    AND CTE.Quantity NOT IN ('N/AV', 'N/AP')
    AND ISNUMERIC(REPLACE(CTE.Quantity,',','')) = 1
    AND NOT EXISTS (SELECT TOP 1 * FROM reports.TMeasurement WHERE TMeasurement.ItemCode = CTE.ItemCode AND TMeasurement.TagRegion = CTE.TagRegion AND TMeasurement.AsAtYear = CTE.AsAtYear)");

            if (years != null && years.Any())
                queryBuilder.Append(" AND CTE.AsAtYear IN (").Append(string.Join(",", years)).Append(")");

            queryBuilder.Append(";");

            var queryString = queryBuilder.ToString();

            using (var context = new InternalDbContext())
            {
                context.Database.CommandTimeout = 240;

                return context.Database.SqlQuery<MeasurementData>(queryString).ToArray();
            }
        }

        #endregion

        #region Read (helper methods)

        private static IQueryable<Field> Filter(IQueryable<Field> query, FieldFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(x => x.Category == filter.Category);

            if (!string.IsNullOrEmpty(filter.Code))
                query = query.Where(x => x.Code.Contains(filter.Code));

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Unit))
                query = query.Where(x => x.Unit == filter.Unit);

            if (filter.IsNumeric.HasValue)
                query = query.Where(x => x.IsNumeric == filter.IsNumeric.Value);

            return query;
        }

        #endregion

        #region Write

        public static Field Insert(Field entity)
        {
            using (var context = new InternalDbContext())
            {
                context.Fields.Add(entity);
                context.SaveChanges();
                return entity;
            }
        }

        public static Field Update(Field entity)
        {
            using (var context = new InternalDbContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
                return entity;
            }
        }

        #endregion

        #region DELETE

        public static void Delete(string code)
        {
            using (var db = new InternalDbContext())
            {
                var entity = new Field { Code = code };
                db.Fields.Attach(entity);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        #region Read (get field data)

        private const BindingFlags GetPropertyBindingFlags = BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase;

        public static FieldMetadataEntity SelectFieldMetadata(string fieldCode)
        {
            #region Query

            const string query = @"
SELECT
    columns.max_length AS [MaxLength]
   ,columns.is_nullable AS IsNullable
FROM
    sys.tables
    INNER JOIN sys.columns ON columns.[object_id] = tables.[object_id]
WHERE
    tables.[name] = LEFT(@FieldCode,2) + 'Program'
    AND columns.[name] = @FieldCode;
";

            #endregion

            using (var db = new InternalDbContext())
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@FieldCode", fieldCode)
                };

                return db.Database.SqlQuery<FieldMetadataEntity>(query, parameters).Single();
            }
        }

        public static FieldDataEntity SelectFieldData(int id, string fieldCode)
        {
            if (fieldCode.Length < 5)
                throw new ApplicationException("Invalid field code: " + fieldCode);

            var tableCode = fieldCode.Substring(0, 2);

            if (string.Equals(tableCode, "AB", StringComparison.OrdinalIgnoreCase))
            {
                var bindExpr = GetBinder<AbProgram>(nameof(AbProgram.AbProgramId), fieldCode);
                return AbProgramRepository.Bind(id, bindExpr);
            }
            else if (string.Equals(tableCode, "HC", StringComparison.OrdinalIgnoreCase))
            {
                var bindExpr = GetBinder<HcProgram>(nameof(HcProgram.HcProgramId), fieldCode);
                return HcProgramRepository.Bind(id, bindExpr);
            }
            else if (string.Equals(tableCode, "HI", StringComparison.OrdinalIgnoreCase))
            {
                var bindExpr = GetBinder<HiProgram>(nameof(HiProgram.HiProgramId), fieldCode);
                return HiProgramRepository.Bind(id, bindExpr);
            }
            else if (string.Equals(tableCode, "MF", StringComparison.OrdinalIgnoreCase))
            {
                var bindExpr = GetBinder<MfProgram>(nameof(MfProgram.MfProgramId), fieldCode);
                return MfProgramRepository.Bind(id, bindExpr);
            }
            else if (string.Equals(tableCode, "MR", StringComparison.OrdinalIgnoreCase))
            {
                var bindExpr = GetBinder<MrProgram>(nameof(MrProgram.MrProgramId), fieldCode);
                return MrProgramRepository.Bind(id, bindExpr);
            }
            else if (string.Equals(tableCode, "PA", StringComparison.OrdinalIgnoreCase))
            {
                var bindExpr = GetBinder<PaProgram>(nameof(PaProgram.PaProgramId), fieldCode);
                return PaProgramRepository.Bind(id, bindExpr);
            }

            throw new ApplicationException("Invalid field code: " + fieldCode);
        }

        public static int CountFieldData(string fieldCode)
        {
            if (fieldCode.Length < 5)
                throw new ApplicationException("Invalid field code: " + fieldCode);

            var tableCode = fieldCode.Substring(0, 2);

            if (string.Equals(tableCode, "AB", StringComparison.OrdinalIgnoreCase))
                return AbProgramRepository.Count(x => true);
            else if (string.Equals(tableCode, "HC", StringComparison.OrdinalIgnoreCase))
                return HcProgramRepository.Count(x => true);
            else if (string.Equals(tableCode, "HI", StringComparison.OrdinalIgnoreCase))
                return HiProgramRepository.Count(x => true);
            else if (string.Equals(tableCode, "MF", StringComparison.OrdinalIgnoreCase))
                return MfProgramRepository.Count(x => true);
            else if (string.Equals(tableCode, "MR", StringComparison.OrdinalIgnoreCase))
                return MrProgramRepository.Count(x => true);
            else if (string.Equals(tableCode, "PA", StringComparison.OrdinalIgnoreCase))
                return PaProgramRepository.Count(x => true);

            throw new ApplicationException("Invalid field code: " + fieldCode);
        }

        private static Expression<Func<TEntity, FieldDataEntity>> GetBinder<TEntity>(string pkName, string fieldCode)
        {
            var entityType = typeof(TEntity);
            var modelType = typeof(FieldDataEntity);

            // TEntity x

            var inputEntityExpr = Expression.Parameter(entityType, "x");

            // x.{PrimaryKey}

            var entityPkProp = entityType.GetProperty(pkName, GetPropertyBindingFlags);
            var getEntityPkExpr = Expression.Property(inputEntityExpr, entityPkProp);

            // x.SurveyYear

            var entitySurveyProp = entityType.GetProperty(nameof(AbProgram.SurveyYear), GetPropertyBindingFlags);
            var getEntitySurveyExpr = Expression.Property(inputEntityExpr, entitySurveyProp);

            // x.StateName

            var entityStateProp = entityType.GetProperty(nameof(AbProgram.StateName), GetPropertyBindingFlags);
            var getEntityStateExpr = Expression.Property(inputEntityExpr, entityStateProp);

            // x.{FieldCode}

            var entityValueProp = entityType.GetProperty(fieldCode, GetPropertyBindingFlags);
            var getEntityValueExpr = Expression.Property(inputEntityExpr, entityValueProp);

            // TModel.{ID} = x.{PrimaryKey}

            var modelPkProp = modelType.GetProperty(nameof(FieldDataEntity.Id), GetPropertyBindingFlags);
            var pkBindExpr = Expression.Bind(modelPkProp, getEntityPkExpr);

            // TModel.{Year} = x.SurveyYear

            var modelSurveyProp = modelType.GetProperty(nameof(FieldDataEntity.Year), GetPropertyBindingFlags);
            var surveyBindExpr = Expression.Bind(modelSurveyProp, getEntitySurveyExpr);

            // TModel.{State} = x.StateName

            var modelStateProp = modelType.GetProperty(nameof(FieldDataEntity.State), GetPropertyBindingFlags);
            var stateBindExpr = Expression.Bind(modelStateProp, getEntityStateExpr);

            // TModel.{Value} = x.{FieldCode}

            var modelValueProp = modelType.GetProperty(nameof(FieldDataEntity.Value), GetPropertyBindingFlags);
            var valueBindExpr = Expression.Bind(modelValueProp, getEntityValueExpr);

            // new TModel { {Year} = x.SurveyYear, {State} = x.StateName, {Value} = x.{FieldCode} }

            var modelCtorExpr = Expression.New(modelType);
            var modelInitExpr = Expression.MemberInit(modelCtorExpr, pkBindExpr, surveyBindExpr, stateBindExpr, valueBindExpr);

            // x => new TModel { TModel.{ID} = x.{PrimaryKey}, {Year} = x.SurveyYear, {State} = x.StateName, {Value} = x.{FieldCode} }

            return Expression.Lambda<Func<TEntity, FieldDataEntity>>(modelInitExpr, inputEntityExpr);
        }

        public static bool IsValid(string fieldCode)
        {
            if (fieldCode.Length < 5)
                return false;

            var tableCode = fieldCode.Substring(0, 2);

            Type entityType;

            if (string.Equals(tableCode, "AB", StringComparison.OrdinalIgnoreCase))
                entityType = typeof(AbProgram);
            else if (string.Equals(tableCode, "HC", StringComparison.OrdinalIgnoreCase))
                entityType = typeof(HcProgram);
            else if (string.Equals(tableCode, "HI", StringComparison.OrdinalIgnoreCase))
                entityType = typeof(HiProgram);
            else if (string.Equals(tableCode, "MF", StringComparison.OrdinalIgnoreCase))
                entityType = typeof(MfProgram);
            else if (string.Equals(tableCode, "MR", StringComparison.OrdinalIgnoreCase))
                entityType = typeof(MrProgram);
            else if (string.Equals(tableCode, "PA", StringComparison.OrdinalIgnoreCase))
                entityType = typeof(PaProgram);
            else
                return false;

            return entityType.GetProperty(fieldCode, GetPropertyBindingFlags) != null;
        }

        #endregion

        #region Write (update field data)

        public static int UpdateFieldData(int id, string fieldCode, FieldDataEntity fieldData)
        {
            const string query = @"
UPDATE
    custom_ncsha.{0}Program
SET
    SurveyYear = @Year
   ,StateName = @State
   ,{1} = @Value
WHERE
    {0}ProgramID = @ID;

SELECT @@ROWCOUNT;";

            var tableCode = fieldCode.Substring(0, 2);
            var currentQuery = string.Format(query, tableCode, fieldCode);

            using (var db = new InternalDbContext())
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@ID", fieldData.Id),
                    new SqlParameter("@Year", fieldData.Year),
                    new SqlParameter("@State", fieldData.State),
                    new SqlParameter("@Value", fieldData.Value)
                };

                return db.Database.SqlQuery<int>(currentQuery, parameters).First();
            }
        }

        #endregion
    }
}
