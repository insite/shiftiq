using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Admin.Reports.Utilities;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using FunctionType = InSite.Domain.Reports.ReportAggregate.FunctionType;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class BuildAggregate : BaseUserControl
    {
        #region Events

        public event BuildViewChangedHandler ViewChanged;
        protected void OnViewChanged(BuildViewType view) =>
            ViewChanged?.Invoke(this, new BuildViewChangedArgs(view));

        #endregion

        #region Properties

        public Build.IData BuildData { get; set; }

        private string DefaultColumnName
        {
            get => (string)ViewState[nameof(DefaultColumnName)];
            set => ViewState[nameof(DefaultColumnName)] = value;
        }

        private int? EditIndex
        {
            get => (int?)ViewState[nameof(EditIndex)];
            set => ViewState[nameof(EditIndex)] = value;
        }

        private List<ReportAggregate> Aggregates
        {
            get => (List<ReportAggregate>)ViewState[nameof(Aggregates)];
            set => ViewState[nameof(Aggregates)] = value;
        }

        public bool HasItems => Aggregates.Count > 0;

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AggregateAliasUniqueValidator.ServerValidate += AggregateAliasUniqueValidator_ServerValidate;

            AggregateRepeater.DataBinding += AggregateRepeater_DataBinding;
            AggregateRepeater.ItemCommand += AggregateRepeater_ItemCommand;

            AggregateFunction.AutoPostBack = true;
            AggregateFunction.ValueChanged += AggregateFunction_ValueChanged;

            AggregateViewSelector.AutoPostBack = true;
            AggregateViewSelector.ValueChanged += AggregateViewSelector_ValueChanged;
        }

        #endregion

        #region Event handlers

        private void AggregateRepeater_DataBinding(object sender, EventArgs e)
        {
            AggregateRepeater.DataSource = Aggregates.Select(x => new
            {
                x.Alias,
                PseudoSql = x.ToString()
            });
        }

        private void AggregateRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAggregate")
            {
                Aggregates.RemoveAt(e.Item.ItemIndex);

                AggregateRepeater.DataBind();
            }
            else if (e.CommandName == "EditAggregate")
            {
                Edit(e.Item.ItemIndex);
            }
        }

        private void AggregateAliasUniqueValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var name = AggregateAlias.Text;
            if (name.IsEmpty())
                return;

            var skipIndex = EditIndex ?? -1;

            for (var i = 0; i < Aggregates.Count; i++)
            {
                if (i != skipIndex && string.Equals(Aggregates[i].Alias, name, StringComparison.OrdinalIgnoreCase))
                {
                    args.IsValid = false;
                    break;
                }
            }
        }

        private void AggregateFunction_ValueChanged(object sender, ComboBoxValueChangedEventArgs e) =>
            OnAggregateFunctionChanged(e.OldValue);

        private void OnAggregateFunctionChanged(string prevValue = null)
        {
            var isColumnNeeded = AggregateFunction.Value.IsNotEmpty()
                && AggregateFunction.Value.ToEnum<FunctionType>().IsRequireColumn();

            AggregateColumnContainer.Visible = isColumnNeeded;
            AggregateViewRequiredValidator.Visible = isColumnNeeded;
            AggregateColumnRequiredValidator.Visible = isColumnNeeded;

            if (prevValue.IsEmpty() || prevValue.ToEnum<FunctionType>().IsRequireColumn() != isColumnNeeded)
            {
                AggregateViewSelector.Value = null;

                OnAggregateViewChanged();
            }
        }

        private void AggregateViewSelector_ValueChanged(object sender, EventArgs e) =>
            OnAggregateViewChanged();

        private void OnAggregateViewChanged()
        {
            var viewAlias = AggregateViewSelector.Value;
            var hasValue = viewAlias.IsNotEmpty();

            AggregateColumnField.Visible = hasValue;
            AggregateColumnSelector.Value = null;

            if (hasValue)
                AggregateColumnSelector.LoadItems(
                    BuildData.DataSet.GetViewColumns(viewAlias)
                        .Select(x => new ReportColumn
                        {
                            Name = viewAlias + "." + x.ColumnName,
                            DataType = x.DataType
                        })
                        .Where(x => x.IsNumeric)
                        .OrderBy(x => x.Name),
                    "Name",
                    "NameWithoutView");
            else
                AggregateColumnSelector.Items.Clear();
        }

        #endregion

        #region Methods

        public void LoadData()
        {
            Aggregates = new List<ReportAggregate>();

            if (BuildData.Aggregates.IsNotEmpty())
                Aggregates.AddRange(BuildData.Aggregates);

            AggregateFunction.LoadItems(
                FunctionType.Count,
                FunctionType.Sum,
                FunctionType.Min,
                FunctionType.Max,
                FunctionType.Mean,
                FunctionType.Median,
                FunctionType.Mode
            );

            AggregateViewSelector.LoadItems(
                BuildData.DataSet.GetViewSelectorItems());

            View();
        }

        public void View()
        {
            EditIndex = null;
            MultiView.SetActiveView(TableView);

            var hasData = Aggregates.Count > 0;

            AggregateRepeater.Visible = hasData;
            AggregateRepeater.DataBind();

            if (!hasData)
                TableStatus.AddMessage(AlertType.Information, "There are no added statistic columns.");

            OnViewChanged(BuildViewType.View);
        }

        public void Edit(int index)
        {
            if (index >= 0 && index < Aggregates.Count)
            {
                EditIndex = index;

                var aggregate = Aggregates[index];
                AggregateAlias.Text = aggregate.Alias;
                AggregateFunction.Value = aggregate.Function.GetName();

                OnAggregateFunctionChanged();

                if (aggregate.Function.IsRequireColumn())
                {
                    AggregateViewSelector.Value = aggregate.Column.ViewAlias;

                    OnAggregateViewChanged();

                    AggregateColumnSelector.Value = aggregate.Column.Name;
                }
            }
            else
            {
                AggregateAlias.Text = null;
                AggregateFunction.Value = null;

                OnAggregateFunctionChanged();
            }

            MultiView.SetActiveView(EditorView);

            OnViewChanged(BuildViewType.Edit);
        }

        public void Save()
        {
            ReportAggregate aggregate;

            if (EditIndex.HasValue)
                aggregate = Aggregates[EditIndex.Value];
            else
                Aggregates.Add(aggregate = new ReportAggregate());

            aggregate.Alias = AggregateAlias.Text.Replace('[', '_').Replace(']', '_');
            aggregate.Function = AggregateFunction.Value.ToEnum<FunctionType>();

            if (aggregate.Function.IsRequireColumn())
            {
                var columnFullName = AggregateColumnSelector.Value;

                ReportHelper.GetNameParts(columnFullName, out var viewAlias, out var columnName);

                var viewFullName = BuildData.DataSet.GetViewName(viewAlias);

                ReportHelper.GetNameParts(viewFullName, out var viewSchema, out var viewName);

                aggregate.Column = new ReportColumn
                {
                    Name = columnFullName,
                };

                if (aggregate.Function != FunctionType.Mode)
                {
                    var columnMeta = VViewColumnSearch.Select(viewSchema, viewName, columnName);
                    aggregate.Column.DataType = columnMeta.DataType;
                }
                else
                    aggregate.Column.DataType = "varchar";
            }
            else
                aggregate.Column = null;

            View();
        }

        public ReportAggregate[] GetAggregates() => Aggregates.ToArray();

        protected override void SetupValidationGroup(string groupName)
        {
            AggregateAliasRequiredValidator.ValidationGroup = groupName;
            AggregateAliasUniqueValidator.ValidationGroup = groupName;
            AggregateFunctionRequiredValidator.ValidationGroup = groupName;
            AggregateViewRequiredValidator.ValidationGroup = groupName;
            AggregateColumnRequiredValidator.ValidationGroup = groupName;
        }

        #endregion
    }
}