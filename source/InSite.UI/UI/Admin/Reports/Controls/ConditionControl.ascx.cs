using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class ConditionControl : BaseUserControl
    {
        private IReadOnlyList<string> BitDataTypes = new List<string> { "bit" };
        private IReadOnlyList<string> DateDataTypes = new List<string> { "date" };
        private IReadOnlyList<string> DateTimeDataTypes = new List<string> { "datetime", "datetimeoffset" };
        private IReadOnlyList<string> NumberDataTypes = new List<string> { "decimal", "float", "int", "money", "numeric", "bigint", "smallmoney" };
        private IReadOnlyList<string> TextDataTypes = new List<string> { "varchar", "nvarchar", "uniqueidentifier" };

        private List<(ReportConditionOperator, string)> BitOperators = new List<(ReportConditionOperator, string)>
        {
            (ReportConditionOperator.Equal, "Is"),
            (ReportConditionOperator.NotEqual, "Is Not")
        };

        private List<(ReportConditionOperator, string)> DateOperators = new List<(ReportConditionOperator, string)>
        {
            (ReportConditionOperator.Less, "<"),
            (ReportConditionOperator.LessOrEqual, "<="),
            (ReportConditionOperator.Greater, ">"),
            (ReportConditionOperator.GreaterOrEqual, ">="),
            (ReportConditionOperator.IsNull, "Is Null"),
            (ReportConditionOperator.IsNotNull, "Is Not Null"),
        };


        private List<(ReportConditionOperator, string)> NumericOperators = new List<(ReportConditionOperator, string)>
        {
            (ReportConditionOperator.Equal, "="),
            (ReportConditionOperator.NotEqual, "<>"),
            (ReportConditionOperator.Less, "<"),
            (ReportConditionOperator.LessOrEqual, "<="),
            (ReportConditionOperator.Greater, ">"),
            (ReportConditionOperator.GreaterOrEqual, ">="),
            (ReportConditionOperator.IsNull, "Is Null"),
            (ReportConditionOperator.IsNotNull, "Is Not Null"),
        };

        private List<(ReportConditionOperator, string)> TextOperators = new List<(ReportConditionOperator, string)>
        {
            (ReportConditionOperator.Contain, "Contains"),
            (ReportConditionOperator.EndWith, "Ends With"),
            (ReportConditionOperator.Equal, "Equals"),
            (ReportConditionOperator.In, "In"),
            (ReportConditionOperator.IsNull, "Is Null"),
            (ReportConditionOperator.IsNotNull, "Is Not Null"),
            (ReportConditionOperator.NotContain, "Does Not Contain"),
            (ReportConditionOperator.NotStartWith, "Does Not Start With"),
            (ReportConditionOperator.NotEndWith, "Does Not End With"),
            (ReportConditionOperator.NotEqual, "Does Not Equal"),
            (ReportConditionOperator.NotIn, "Not In"),
            (ReportConditionOperator.StartWith, "Starts With"),
        };

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ConditionGrid.ItemDataBound += ConditionGrid_ItemDataBound;

            ConditionClause.LoadItems(
                ReportConditionClauseType.And,
                ReportConditionClauseType.Or);
        }

        public void LoadData(IEnumerable<ReportColumn> columns, ReportConditionItemList list)
        {
            ConditionClause.Value = list != null
                ? list.ConditionClause.GetName()
                : ReportConditionClauseType.And.GetName();

            ConditionGrid.DataSource = columns.Select(column =>
            {
                return list?.Find(y => y.Column.Name == column.Name) ?? new ReportConditionItem { Column = column };
            });
            ConditionGrid.DataBind();
        }

        private void ConditionGrid_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var condition = (ReportConditionItem)e.Item.DataItem;

            var conditionOperator = (ComboBox)e.Item.FindControl("ConditionOperator");
            conditionOperator.LoadItems(GetConditionOperators(condition.Column.DataType));

            if (condition.Operator != ReportConditionOperator.None)
                conditionOperator.Value = OperatorToString(condition.Operator, condition.Column.DataType);

            SetConditionValue(e.Item, condition.Column.DataType, condition.Value);
        }

        private List<string> GetConditionOperators(string dataType)
        {
            if (BitDataTypes.Contains(dataType))
            {
                return BitOperators.Select(x => x.Item2).ToList();
            }
            else if (DateDataTypes.Contains(dataType) || DateTimeDataTypes.Contains(dataType))
            {
                return DateOperators.Select(x => x.Item2).ToList();
            }
            else if (NumberDataTypes.Contains(dataType))
            {
                return NumericOperators.Select(x => x.Item2).ToList();
            }
            else if (TextDataTypes.Contains(dataType))
            {
                return TextOperators.Select(x => x.Item2).ToList();
            }

            return new List<string>();
        }

        private string OperatorToString(ReportConditionOperator op, string dataType)
        {
            List<(ReportConditionOperator, string)> operators;
            if (BitDataTypes.Contains(dataType))
                operators = BitOperators;
            else if (DateDataTypes.Contains(dataType) || DateTimeDataTypes.Contains(dataType))
                operators = DateOperators;
            else if (NumberDataTypes.Contains(dataType))
                operators = NumericOperators;
            else if (TextDataTypes.Contains(dataType))
                operators = TextOperators;
            else
                return null;

            var index = operators.FindIndex(x => x.Item1 == op);
            return index >= 0 ? operators[index].Item2 : null;
        }

        private ReportConditionOperator StringToOperator(string str, string dataType)
        {
            List<(ReportConditionOperator, string)> operators;

            if (BitDataTypes.Contains(dataType))
                operators = BitOperators;
            else if (DateDataTypes.Contains(dataType) || DateTimeDataTypes.Contains(dataType))
                operators = DateOperators;
            else if (NumberDataTypes.Contains(dataType))
                operators = NumericOperators;
            else if (TextDataTypes.Contains(dataType))
                operators = TextOperators;
            else
                return ReportConditionOperator.None;

            var index = operators.FindIndex(x => x.Item2 == str);
            return index >= 0 ? operators[index].Item1 : ReportConditionOperator.None;
        }

        private void SetConditionValue(RepeaterItem item, string dataType, string value = null)
        {
            var conditionValueBool = (ComboBox)item.FindControl("ConditionValueBool");
            var conditionValueDate = (DateSelector)item.FindControl("ConditionValueDate");
            var conditionValueDateTime = (DateTimeOffsetSelector)item.FindControl("ConditionValueDateTime");
            var conditionValueNumber = (INumericBox)item.FindControl("ConditionValueNumber");
            var conditionValueText = (ITextBox)item.FindControl("ConditionValueText");

            if (BitDataTypes.Contains(dataType))
            {
                conditionValueBool.Visible = true;
                conditionValueBool.ValueAsBoolean = value.HasNoValue() ? (bool?)null : value == "true" || value == "1";
            }
            else if (DateDataTypes.Contains(dataType))
            {
                conditionValueDate.Visible = true;
                if (value.HasValue()) conditionValueDate.Value = DateTime.Parse(value);
            }
            else if (DateTimeDataTypes.Contains(dataType))
            {
                conditionValueDateTime.Visible = true;
                if (value.HasValue()) conditionValueDateTime.Value = DateTimeOffset.Parse(value);
            }
            else if (NumberDataTypes.Contains(dataType))
            {
                conditionValueNumber.Visible = true;
                if (value.HasValue()) conditionValueNumber.ValueAsText = value;
            }
            else if (TextDataTypes.Contains(dataType))
            {
                conditionValueText.Visible = true;
                if (value.HasValue()) conditionValueText.Text = value;
            }
        }

        public void GetCondition(IEnumerable<ReportColumn> columns, ReportConditionItemList list)
        {
            list.Clear();
            list.ConditionClause = ConditionClause.Value.ToEnum<ReportConditionClauseType>();

            foreach (RepeaterItem item in ConditionGrid.Items)
            {
                var columnName = ((System.Web.UI.WebControls.Literal)item.FindControl("ColumnName")).Text;
                var conditionOperator = ((ComboBox)item.FindControl("ConditionOperator")).Value;
                var conditionValue = GetConditionValue(item);

                var column = columns.First(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                var conditionItem = new ReportConditionItem
                {
                    Column = column,
                    Operator = StringToOperator(conditionOperator, column.DataType),
                    Value = conditionValue,
                };

                if (conditionItem.GetSql() != null)
                    list.Add(conditionItem);
            }
        }

        private string GetConditionValue(RepeaterItem item)
        {
            var conditionValueBool = (ComboBox)item.FindControl("ConditionValueBool");
            var conditionValueDate = (DateSelector)item.FindControl("ConditionValueDate");
            var conditionValueDateTime = (DateTimeOffsetSelector)item.FindControl("ConditionValueDateTime");
            var conditionValueNumber = (INumericBox)item.FindControl("ConditionValueNumber");
            var conditionValueText = (ITextBox)item.FindControl("ConditionValueText");

            if (conditionValueBool.Visible)
            {
                var value = conditionValueBool.ValueAsBoolean;
                return !value.HasValue ? null : value.Value ? "1" : "0";
            }
            else if (conditionValueDate.Visible)
            {
                return conditionValueDate.Value?.ToString("yyyy-MM-dd");
            }
            else if (conditionValueDateTime.Visible)
            {
                return conditionValueDateTime.Value?.ToString("yyyy-MM-dd HH:mm:ss zzz");
            }
            else if (conditionValueNumber.Visible)
            {
                return conditionValueNumber.ValueAsText;
            }
            else if (conditionValueText.Visible)
            {
                return conditionValueText.Text;
            }

            return null;
        }
    }
}