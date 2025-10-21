using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Admin.Reports.Utilities;

using Shift.Common;
using Shift.Constant;

using ListItem = Shift.Common.ListItem;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class BuildColumns : BaseUserControl
    {
        #region Constants

        private static readonly ListItem StatisticItem = new ListItem
        {
            Text = "Statistic",
            Value = ReportAggregate.ViewAlias
        };

        #endregion

        #region Properties

        public Build.IData BuildData { get; set; }

        public bool HasSelection => SelectedColumns.Count > 0;

        private List<string> SelectedColumns
        {
            get => (List<string>)(ViewState[nameof(SelectedColumns)]
                ?? (ViewState[nameof(SelectedColumns)] = new List<string>()));
            set => ViewState[nameof(SelectedColumns)] = value;
        }

        #endregion

        #region Fields

        private static readonly Regex ReorderColumnDataPattern = new Regex("(?:(?<From>\\d+):(?<To>\\d+);)", RegexOptions.Compiled);

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ViewSelector.AutoPostBack = true;
            ViewSelector.ValueChanged += ViewSelector_ValueChanged;

            SelectedColumnRepeater.ItemCommand += SelectedColumnRepeater_ItemCommand;

            SelectedColumnRepeater.DataBinding += SelectedColumnRepeater_DataBinding;

            SelectAll.AutoPostBack = true;
            SelectAll.CheckedChanged += SelectAll_CheckedChanged;

            ClearColumnSelectionButton.Click += ClearColumnSelectionButton_Click;
            ReorderColumnSaveButton.Click += ReorderColumnSaveButton_Click;
        }

        #endregion

        #region Event handlers

        private void SelectedColumnRepeater_DataBinding(object sender, EventArgs e)
        {
            SelectedColumnRepeater.DataSource = SelectedColumns;
            ReorderColumnStartButton.Visible = SelectedColumns.Count > 1;
        }

        private void ReorderColumnSaveButton_Click(object sender, EventArgs e)
        {
            if (ReorderColumns())
            {
                SelectedColumnRepeater.DataBind();

                ReorderColumnState.Value = null;
            }
            else
            {
                ControlStatus.AddMessage(AlertType.Error, Translate("Reorder can not be saved."));
            }
        }

        private void ViewSelector_ValueChanged(object sender, EventArgs e) => OnViewSelectorChanged();

        private void OnViewSelectorChanged()
        {
            var value = ViewSelector.Value;
            var hasValue = value.IsNotEmpty();

            ViewPanel.Visible = hasValue;

            if (!hasValue)
                return;

            var columns = value == StatisticItem.Value
                ? BuildData.Aggregates.Select(x => new VViewColumn { ColumnName = x.Alias })
                : BuildData.DataSet.GetViewColumns(value);

            ColumnRepeater.DataSource = columns.Select(x => new
            {
                Text = x.ColumnName == "PersonCode"
                    ? LabelHelper.GetLabelContentText("Person Code").Replace(" ", "")
                    : x.ColumnName,
                Value = value + "." + x.ColumnName
            });
            ColumnRepeater.DataBind();

            var selectedCount = 0;

            foreach (RepeaterItem item in ColumnRepeater.Items)
            {
                var isSelected = (ICheckBox)item.FindControl("IsSelected");
                if (isSelected.Checked = SelectedColumns.Contains(isSelected.Value))
                    selectedCount++;
            }

            SelectAll.Checked = selectedCount == ColumnRepeater.Items.Count;
        }

        private void SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            var isSelectAll = SelectAll.Checked;

            foreach (RepeaterItem item in ColumnRepeater.Items)
            {
                var isSelected = (ICheckBox)item.FindControl("IsSelected");

                isSelected.Checked = isSelectAll;
            }

            OnColumnRepeaterChanged();
        }

        protected void IsSelected_CheckedChanged(object sender, EventArgs e) => OnColumnRepeaterChanged();

        private void OnColumnRepeaterChanged()
        {
            var viewAlias = ViewSelector.Value;

            if (viewAlias.IsNotEmpty())
            {
                foreach (RepeaterItem item in ColumnRepeater.Items)
                {
                    var isSelected = (ICheckBox)item.FindControl("IsSelected");
                    var columnValue = isSelected.Value;

                    if (!isSelected.Checked)
                    {
                        if (SelectedColumns.Contains(columnValue))
                            SelectedColumns.Remove(columnValue);

                        continue;
                    }

                    if (SelectedColumns.Contains(columnValue))
                        continue;

                    var columnName = ReportHelper.GetNamePart(columnValue, 1);

                    if (SelectedColumns.Any(x => x.EndsWith($".{columnName}")))
                    {
                        isSelected.Checked = false;

                        ControlStatus.AddMessage(
                            AlertType.Error,
                            $"{Translate("You can not add")} {columnName} {Translate("column twice")}.");

                        continue;
                    }

                    if (viewAlias != ReportAggregate.ViewAlias)
                    {
                        var views = SelectedColumns
                            .Select(x => x.Remove(x.LastIndexOf('.')))
                            .Where(x => x != ReportAggregate.ViewAlias)
                            .Distinct().ToList();

                        if ((views.Count == 2 && !views.Contains(viewAlias)) || views.Count > 2)
                        {
                            isSelected.Checked = false;

                            ControlStatus.AddMessage(
                                AlertType.Error,
                                Translate("You can not select columns from more than 2 tables."));

                            continue;
                        }
                    }

                    SelectedColumns.Add(columnValue);
                }

                SelectedColumnRepeater.DataBind();
            }
            else
            {
                SelectedColumnRepeater.DataSource = null;
                SelectedColumnRepeater.DataBind();

                ReorderColumnStartButton.Visible = false;
            }
        }

        private void SelectedColumnRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteColumn")
            {
                var column = e.CommandArgument.ToString();

                if (SelectedColumns.Contains(column))
                    SelectedColumns.Remove(column);

                SelectedColumnRepeater.DataBind();

                foreach (RepeaterItem item in ColumnRepeater.Items)
                {
                    var isSelected = (ICheckBox)item.FindControl("IsSelected");
                    if (isSelected.Checked && column == isSelected.Value)
                        isSelected.Checked = false;
                }
            }
        }

        private void ClearColumnSelectionButton_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in ColumnRepeater.Items)
            {
                var isSelected = (ICheckBox)item.FindControl("IsSelected");

                isSelected.Checked = false;
            }

            SelectAll.Checked = false;

            SelectedColumns.Clear();

            SelectedColumnRepeater.DataSource = null;
            SelectedColumnRepeater.DataBind();

            ReorderColumnStartButton.Visible = false;
        }

        #endregion

        #region Methods

        public void LoadData()
        {
            var viewSelectorItems = BuildData.DataSet.GetViewSelectorItems().ToList();

            if (BuildData.Aggregates.Length > 0)
                viewSelectorItems.Add(StatisticItem.Clone());

            ViewSelector.LoadItems(viewSelectorItems);

            ReorderColumnStartButton.Visible = false;

            SelectedColumns = BuildData.Columns.IsNotEmpty()
                ? new List<string>(BuildData.Columns.Select(x => x.Name))
                : new List<string>();

            OnViewSelectorChanged();
            OnColumnRepeaterChanged();
        }

        public ReportColumn[] GetColumns()
        {
            var list = new List<ReportColumn>();

            foreach (var columnFullName in SelectedColumns)
            {
                ReportHelper.GetNameParts(columnFullName, out var viewAlias, out var columnName);

                if (viewAlias == ReportAggregate.ViewAlias)
                {
                    var aggregate = BuildData.Aggregates.Where(x => x.Alias == columnName).FirstOrDefault();
                    if (aggregate != null)
                    {
                        list.Add(new ReportColumn
                        {
                            Name = columnFullName,
                            DataType = aggregate.Column != null ? aggregate.Column.DataType : "int"
                        });
                    }
                }
                else
                {
                    var viewFullName = BuildData.DataSet.GetViewName(viewAlias);

                    ReportHelper.GetNameParts(viewFullName, out var viewSchema, out var viewName);

                    var columnMeta = VViewColumnSearch.Select(viewSchema, viewName, columnName);

                    list.Add(new ReportColumn
                    {
                        Name = columnFullName,
                        DataType = columnMeta.DataType
                    });
                }
            }

            return list.ToArray();
        }

        private bool ReorderColumns()
        {
            if (string.IsNullOrEmpty(ReorderColumnState.Value))
                return true;

            var matches = ReorderColumnDataPattern.Matches(ReorderColumnState.Value);
            if (matches.Count == 0)
                return false;

            var oldColumns = SelectedColumns;
            var columns = new string[SelectedColumns.Count];

            if (matches.Count == oldColumns.Count)
            {
                foreach (Match match in matches)
                {
                    var fromIndex = int.Parse(match.Groups["From"].Value);
                    if (fromIndex < 0 || fromIndex >= oldColumns.Count)
                        return false;

                    var toIndex = int.Parse(match.Groups["To"].Value);
                    if (toIndex < 0 || toIndex >= columns.Length)
                        return false;

                    columns[toIndex] = oldColumns[fromIndex];
                }

                SelectedColumns = columns.ToList();
            }

            return true;
        }

        #endregion
    }
}