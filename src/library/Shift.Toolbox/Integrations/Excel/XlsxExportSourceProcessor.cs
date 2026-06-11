using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Shift.Common;

namespace Shift.Toolbox
{
    internal class XlsxExportSourceProcessor
    {
        #region Fields

        private bool _isDataTable;
        private Func<XlsxExportHelper, DataRow, object[]> _mapRowFunc;
        private Func<XlsxExportHelper, object, object[]> _mapObjectFunc;
        private Type[] _columnTypes;
        private XlsxExportHelper _helper;

        #endregion

        #region Construction

        private XlsxExportSourceProcessor(XlsxExportHelper helper)
        {
            _helper = helper;
        }

        #endregion

        #region Initialization

        public static XlsxExportSourceProcessor Build(XlsxExportHelper helper, object dataSample, List<XlsxExportMappingInfo> mappings)
        {
            if (mappings.IsEmpty())
                throw new ArgumentException("No mappings defined.");

            var result = new XlsxExportSourceProcessor(helper);

            if (dataSample is DataRowView rowView)
            {
                var sampleColumns = rowView?.DataView.Table.Columns.Cast<DataColumn>()
                    .ToDictionary(x => x.ColumnName, StringComparer.OrdinalIgnoreCase);

                if (sampleColumns.IsEmpty())
                    throw new ArgumentException("The data source has no columns.");

                result._isDataTable = true;
                result._mapRowFunc = BuildDataTableMapping(sampleColumns, mappings);
                result._columnTypes = BuildColumnTypes(mappings.Select(x => sampleColumns[x.PropertyName].DataType));
            }
            else
            {
                var sampleType = dataSample.GetType();
                var sampleProps = sampleType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

                if (sampleProps.IsEmpty())
                    throw new ArgumentException("The data source has no properties.");

                result._isDataTable = false;
                result._mapObjectFunc = BuildObjectMapping(dataSample.GetType(), sampleProps, mappings);
                result._columnTypes = BuildColumnTypes(mappings.Select(x => sampleProps[x.PropertyName].PropertyType));
            }

            return result;
        }

        private static Func<XlsxExportHelper, DataRow, object[]> BuildDataTableMapping(Dictionary<string, DataColumn> dataColumns, List<XlsxExportMappingInfo> mappings)
        {
            var helperType = typeof(XlsxExportHelper);
            var expressions = new List<Expression>();

            // XlsxExportHelper.GetRowValue(object)
            var methodHelperGetValue = helperType.GetMethod(
                "GetRowValue",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(object) },
                null);

            // XlsxExportHelper helper
            var paramHelper = Expression.Parameter(helperType, "helper");

            // DataRow row
            var paramRow = Expression.Parameter(typeof(DataRow), "row");

            // object[] array
            var varArray = Expression.Variable(typeof(object[]), "array");
            {
                // new object[N]
                var createArray = Expression.NewArrayBounds(typeof(object), Expression.Constant(mappings.Count));

                // object[] array = new object[N]
                var assignArray = Expression.Assign(varArray, createArray);

                expressions.Add(assignArray);
            }

            for (var i = 0; i < mappings.Count; i++)
            {
                var map = mappings[i];

                if (!dataColumns.TryGetValue(map.PropertyName, out var column))
                    throw new InvalidOperationException($"Invalid property name: {map.PropertyName}");

                // array[N]
                var accessArrayIndex = Expression.ArrayAccess(varArray, Expression.Constant(i));

                // row[N]
                var accessRowIndexer = (Expression)Expression.Property(paramRow, "Item", Expression.Constant(column.Ordinal));

                // XlsxExportHelper.GetRowValue(row[N])
                var callGetValue = Expression.Call(paramHelper, methodHelperGetValue, accessRowIndexer);

                // array[N] = XlsxExportHelper.GetRowValue(row[N])
                var assignArrayIndex = Expression.Assign(accessArrayIndex, callGetValue);

                expressions.Add(assignArrayIndex);
            }

            // return array
            expressions.Add(varArray);

            var block = Expression.Block(new[] { varArray }, expressions);
            var lambda = Expression.Lambda<Func<XlsxExportHelper, DataRow, object[]>>(block, paramHelper, paramRow);

            return lambda.Compile();
        }

        private static Func<XlsxExportHelper, object, object[]> BuildObjectMapping(Type dataType, Dictionary<string, PropertyInfo> dataProperties, List<XlsxExportMappingInfo> mappings)
        {
            var helperType = typeof(XlsxExportHelper);
            var expressions = new List<Expression>();

            // XlsxExportHelper.GetExportDate(DateTimeOffset?)
            var methodHelperGetDate = helperType.GetMethod(
                "GetExportDate",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(DateTimeOffset?) },
                null);

            // XlsxExportHelper helper
            var paramHelper = Expression.Parameter(helperType, "helper");

            // object obj
            var paramObj = Expression.Parameter(typeof(object), "obj");

            // DataType data
            var varData = Expression.Variable(dataType, "data");
            {
                // (DataType)obj
                var convertObj = Expression.Convert(paramObj, dataType);

                // object[] array = new object[N]
                var assignData = Expression.Assign(varData, convertObj);

                expressions.Add(assignData);
            }

            // object[] array
            var varArray = Expression.Variable(typeof(object[]), "array");
            {
                // new object[N]
                var createArray = Expression.NewArrayBounds(typeof(object), Expression.Constant(mappings.Count));

                // object[] array = new object[N]
                var assignArray = Expression.Assign(varArray, createArray);

                expressions.Add(assignArray);
            }

            for (var i = 0; i < mappings.Count; i++)
            {
                var map = mappings[i];

                if (!dataProperties.TryGetValue(map.PropertyName, out var prop))
                    throw new InvalidOperationException($"Invalid property name: {map.PropertyName}");

                var propType = prop.PropertyType;

                // array[N]
                var accessArrayIndex = Expression.ArrayAccess(varArray, Expression.Constant(i));

                // data.{Property}
                var accessDataProp = (Expression)Expression.MakeMemberAccess(varData, prop);

                if (propType == typeof(DateTimeOffset) || propType == typeof(DateTimeOffset?))
                {
                    // helper.GetExportDate((DateTimeOffset?)data.{Property})
                    accessDataProp = Expression.Call(paramHelper, methodHelperGetDate, Expression.Convert(accessDataProp, typeof(DateTimeOffset?)));
                }
                else if (propType.IsValueType)
                {
                    // (object)data.{Property}
                    accessDataProp = Expression.Convert(accessDataProp, typeof(object));
                }

                // array[N] = data.{Property}
                var assignArrayIndex = Expression.Assign(accessArrayIndex, accessDataProp);

                expressions.Add(assignArrayIndex);
            }

            // return array
            expressions.Add(varArray);

            var block = Expression.Block(new[] { varData, varArray }, expressions);
            var lambda = Expression.Lambda<Func<XlsxExportHelper, object, object[]>>(block, paramHelper, paramObj);

            return lambda.Compile();
        }

        private static Type[] BuildColumnTypes(IEnumerable<Type> types)
        {
            var result = types.ToArray();

            for (var i = 0; i < result.Length; i++)
            {
                var t = result[i];
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    result[i] = Nullable.GetUnderlyingType(t);
            }

            return result;
        }

        #endregion

        #region Methods

        public object[][] Map(IEnumerable source)
        {
            var dataItems = new List<object[]>();

            if (_isDataTable)
            {
                if (source is DataView dataView)
                {
                    foreach (DataRowView rowView in dataView)
                        dataItems.Add(_mapRowFunc(_helper, rowView.Row));
                }
                else if (source is DataTable dataTable)
                {
                    foreach (DataRow row in dataTable.Rows)
                        dataItems.Add(_mapRowFunc(_helper, row));
                }
                else
                {
                    foreach (DataRow row in source)
                        dataItems.Add(_mapRowFunc(_helper, row));
                }
            }
            else
            {
                foreach (var obj in source)
                    dataItems.Add(_mapObjectFunc(_helper, obj));
            }

            return dataItems.ToArray();
        }

        public Type GetColumnType(int index)
        {
            return _columnTypes[index];
        }

        #endregion
    }
}
