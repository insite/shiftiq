using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Reports
{
    [Serializable]
    public class ReportConditionItem
    {
        #region Classes

        private class OperatorDescriptor
        {
            public bool NeedValue { get; }
            public Func<ReportColumn, string, bool?, string> Handler { get; }

            public OperatorDescriptor(bool needValue, Func<ReportColumn, string, bool?, string> handler)
            {
                NeedValue = needValue;
                Handler = handler;
            }
        }

        #endregion

        #region Constants

        private const string LikeEscapeChar = @"\";

        private static string[] WildcartChars = new string[] { " % ", "_", "[", "]" };

        #endregion

        #region Fields

        private static Dictionary<ReportConditionOperator, OperatorDescriptor> Descriptors = new Dictionary<ReportConditionOperator, OperatorDescriptor>
        {
            { ReportConditionOperator.IsNull,         new OperatorDescriptor(false, (c, v, s) => c.IsText ? $"({GetColumnName(c, s)} IS NULL OR {GetColumnName(c, s)} = '')" : $"{GetColumnName(c, s)} IS NULL") },
            { ReportConditionOperator.IsNotNull,      new OperatorDescriptor(false, (c, v, s) => c.IsText ? $"({GetColumnName(c, s)} IS NOT NULL AND {GetColumnName(c, s)} != '')" : $"{GetColumnName(c, s)} IS NOT NULL") },
            { ReportConditionOperator.Equal,          new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} = {AdjustValue(c, v)}") },
            { ReportConditionOperator.NotEqual,       new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} <> {AdjustValue(c, v)}") },
            { ReportConditionOperator.In,             new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} IN {PrepareMultipleValues(c, v)}") },
            { ReportConditionOperator.NotIn,          new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} NOT IN {PrepareMultipleValues(c, v)}") },
            { ReportConditionOperator.Less,           new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} < {AdjustValue(c, v)}") },
            { ReportConditionOperator.LessOrEqual,    new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} <= {AdjustValue(c, v)}") },
            { ReportConditionOperator.Greater,        new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} > {AdjustValue(c, v)}") },
            { ReportConditionOperator.GreaterOrEqual, new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} >= {AdjustValue(c, v)}") },
            { ReportConditionOperator.StartWith,      new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} LIKE '{EscapeValue(v)}%' ESCAPE '{LikeEscapeChar}'") },
            { ReportConditionOperator.Contain,        new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} LIKE '%{EscapeValue(v)}%' ESCAPE '{LikeEscapeChar}'") },
            { ReportConditionOperator.EndWith,        new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} LIKE '%{EscapeValue(v)}' ESCAPE '{LikeEscapeChar}'") },
            { ReportConditionOperator.NotStartWith,   new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} NOT LIKE '{EscapeValue(v)}%' ESCAPE '{LikeEscapeChar}'") },
            { ReportConditionOperator.NotContain,     new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} NOT LIKE '%{EscapeValue(v)}%' ESCAPE '{LikeEscapeChar}'") },
            { ReportConditionOperator.NotEndWith,     new OperatorDescriptor(true, (c, v, s)  => $"{GetColumnName(c, s)} NOT LIKE '%{EscapeValue(v)}' ESCAPE '{LikeEscapeChar}'") },
        };

        private ReportColumn _column;

        #endregion

        #region Properties

        [JsonIgnore]
        public ReportColumn Column
        {
            get => _column;
            set
            {
                _column = value;
                ColumnName = value?.Name;
            }
        }

        [JsonProperty("Column")]
        internal string ColumnName { get; set; }

        public ReportConditionOperator Operator { get; set; }

        public string Value { get; set; }

        #endregion

        #region Methods (public)

        public string GetSql(bool? isStatistic = false)
        {
            if (Operator == ReportConditionOperator.None)
                return null;

            var descriptor = Descriptors[Operator];
            if (descriptor.NeedValue && string.IsNullOrWhiteSpace(Value))
                return null;

            return descriptor.Handler(Column, Value, isStatistic);
        }

        #endregion

        #region Methods (private)

        private static string GetColumnName(ReportColumn column, bool? isStatistic)
        {
            return isStatistic == true ? "[" + column.Name + "]" : column.Name;
        }

        private static string EscapeValue(string value)
        {
            var adjust = value.Replace("'", "''");

            if (!adjust.Contains(LikeEscapeChar)
                && !StringHelper.ContainsAny(adjust, WildcartChars)
                )
            {
                return adjust;
            }

            adjust = adjust.Replace(LikeEscapeChar, $@"{LikeEscapeChar}{LikeEscapeChar}");

            foreach (var c in WildcartChars)
                adjust = adjust.Replace(c, $@"{LikeEscapeChar}{c}");

            return adjust;
        }

        private static string AdjustValue(ReportColumn column, string value)
        {
            if (column.IsNumeric || column.IsBit)
            {
                double.Parse(value); //Make sure this is actually numeric value
                return value;
            }

            var adjust = value.Replace("'", "''");

            return $"'{adjust}'";
        }

        private static string PrepareMultipleValues(ReportColumn column, string value)
        {
            var sb = new StringBuilder();
            sb.Append("(");
            var list = CsvConverter.CsvTextToList(value);
            for (var i = 0; i < list.Length; i++)
            {
                var item = list[i];
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                if (i > 0)
                    sb.Append(",");

                if (column.IsNumeric || column.IsBit)
                {
                    double.Parse(item);
                    sb.Append(item);
                }
                else
                {
                    item = item.Replace("'", "''");
                    sb.Append($"'{item}'");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        #endregion

        #region Methods (serialization)

        public bool ShouldSerializeValue() => Value.IsNotEmpty();

        #endregion
    }
}