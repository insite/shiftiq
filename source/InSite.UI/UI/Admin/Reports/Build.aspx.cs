using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.UI.Admin.Reports
{
    public partial class Build : AdminBasePage, IHasParentLinkParameters
    {
        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (parent.Name.EndsWith("/edit"))
            {
                var reportId = ReportIdentifier;
                if (reportId.HasValue)
                    return $"id={reportId}";
            }

            return null;
        }

        #endregion

        #region Classes

        public interface IData
        {
            ReportDataSource DataSet { get; }
            ReportAggregate[] Aggregates { get; }
            ReportColumn[] Columns { get; }
            ReportCondition[] Conditions { get; }
        }

        [Serializable]
        private class Data : IData
        {
            public ReportDataSource DataSet { get; set; }
            public ReportAggregate[] Aggregates { get; set; }
            public ReportColumn[] Columns { get; set; }
            public ReportCondition[] Conditions { get; set; }
        }

        #endregion

        #region Properties

        private Guid? ReportIdentifier => Guid.TryParse(Request["id"], out Guid result) ? result : (Guid?)null;

        private bool IsExecute => bool.TryParse(Request["execute"], out bool result) ? result : false;

        private ReportDefinition Report
        {
            get => (ReportDefinition)ViewState[nameof(Report)];
            set => ViewState[nameof(Report)] = value;
        }

        public static string CreateReportJson
        {
            get => (string)HttpContext.Current.Session[nameof(CreateReportJson)];
            set => HttpContext.Current.Session[nameof(CreateReportJson)] = value;
        }

        private Data ControlData
        {
            get => (Data)ViewState[nameof(ControlData)];
            set => ViewState[nameof(ControlData)] = value;
        }

        private string ReportTitle
        {
            get => (string)ViewState[nameof(ReportTitle)];
            set => ViewState[nameof(ReportTitle)] = value;
        }

        #endregion

        #region Initalization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NextButtonDataSet.Click += NextButtonDataSet_Click;
            NextButtonAggregate.Click += NextButtonAggregate_Click;
            NextButtonColumn.Click += NextButtonColumn_Click;
            NextButtonCondition.Click += NextButtonCondition_Click;

            ColumnRequiredValidator.ServerValidate += ColumnRequiredValidator_ServerValidate;

            Aggregates.ViewChanged += Aggregates_ViewChanged;
            AddAggregateButton.Click += AddAggregateButton_Click;
            SaveAggregateButton.Click += SaveAggregateButton_Click;
            CancelSaveAggregateButton.Click += CancelSaveAggregateButton_Click;

            Conditions.ViewChanged += Conditions_ViewChanged;
            AddConditionButton.Click += AddConditionButton_Click;
            SaveConditionButton.Click += SaveConditionButton_Click;
            CancelSaveConditionButton.Click += CancelSaveConditionButton_Click;

            ResultCondition.AutoPostBack = true;
            ResultCondition.ValueChanged += ResultCondition_ValueChanged;

            ResultDownloadButton.Click += ResultDownloadButton_Click;

            SaveReportButton.Click += SaveReportButton_Click;

            ResultPagination.PageChanged += ResultPagination_PageChanged;
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                ControlData = new Data();

            Aggregates.BuildData = ControlData;
            Columns.BuildData = ControlData;
            Conditions.BuildData = ControlData;

            if (!IsPostBack)
            {
                Open();

                PageHelper.AutoBindHeader(this, null, ReportTitle);
            }
        }

        private void Open()
        {
            ReportTitle = null;

            if (ReportIdentifier == null)
                return;

            var report = VReportSearch.Select(ReportIdentifier.Value);
            if (report == null)
            {
                HttpResponseHelper.SendHttp404();
                return;
            }

            if (!IsExecute && report.OrganizationIdentifier != Organization.Identifier
                || !VReportSearch.HasPermissions(report, Organization.Identifier, User.UserIdentifier)
                )
            {
                HttpResponseHelper.SendHttp403();
                return;
            }

            ReportTitle = report.ReportTitle;

            if (report.ReportData.IsNotEmpty())
                LoadReport(report.ReportData);
        }

        private void LoadReport(string json)
        {
            try
            {
                var reportDefinition = ReportDefinition.Deserialize(json);
                var dataSource = reportDefinition != null ? ReportDataSourceReader.ReadDataSource(reportDefinition.DataSource) : null;

                if (dataSource == null)
                    throw new Exception("The data source for this report has been modified. Please re-create your report");

                DataSetSelector.SelectedValue = reportDefinition.DataSource;

                ControlData.DataSet = DataSetSelector.GetSelectedDataSet();
                ControlData.Aggregates = reportDefinition.Aggregates.ToArray();
                ControlData.Columns = reportDefinition.Columns.ToArray();
                ControlData.Conditions = reportDefinition.Conditions.ToArray();

                Aggregates.LoadData();
                Columns.LoadData();
                Conditions.LoadData();

                if (IsExecute)
                {
                    SelectSection(ResultSection);
                    BindResultCondition(reportDefinition.Conditions);
                    OnResultConditionChanged();
                }
                else
                {
                    SelectSection(ConditionSection);
                }
            }
            catch (Exception ex)
            {
                Status.AddMessage(AlertType.Error, $"{Translate("Can't load report")}. {ex.Message}");
            }
        }

        #endregion

        #region Event handlers

        private void NextButtonDataSet_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var dataSet = DataSetSelector.GetSelectedDataSet();

            if (ControlData.DataSet == null || ControlData.DataSet.Name != dataSet.Name)
            {
                ControlData.DataSet = dataSet;
                ControlData.Aggregates = null;
                ControlData.Columns = null;
                ControlData.Conditions = null;
            }

            Aggregates.LoadData();

            if (DataSetStatistic.Checked)
                SelectSection(AggregateSection);
            else
                NextButtonAggregate_Click(sender, e);
        }

        private void NextButtonAggregate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ControlData.Aggregates = Aggregates.GetAggregates();

            var validAggs = ControlData.Aggregates
                .Select(y => ReportAggregate.ViewAlias + "." + y.Alias)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (ControlData.Columns.IsNotEmpty())
                ControlData.Columns = ControlData.Columns
                    .Where(x => !x.IsStatistic || validAggs.Contains(x.Name))
                    .ToArray();

            Columns.LoadData();

            SelectSection(ColumnSection);
        }

        private void Aggregates_ViewChanged(object sender, BuildViewChangedArgs args)
        {
            var isEdit = args.View == BuildViewType.Edit;

            AddAggregateButton.Visible = !isEdit;
            NextButtonAggregate.Visible = !isEdit;

            SaveAggregateButton.Visible = isEdit;
            CancelSaveAggregateButton.Visible = isEdit;
        }

        private void AddAggregateButton_Click(object sender, EventArgs e)
        {
            Aggregates.Edit(-1);
        }

        private void SaveAggregateButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Aggregates.Save();
        }

        private void CancelSaveAggregateButton_Click(object sender, EventArgs e)
        {
            Aggregates.View();
        }

        private void ColumnRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Columns.HasSelection;
        }

        private void NextButtonColumn_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ControlData.Columns = Columns.GetColumns();

            var validCols = ControlData.Columns
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (ControlData.Conditions.IsNotEmpty())
            {
                foreach (var condition in ControlData.Conditions)
                    condition.RemoveColumns(x => !validCols.Contains(x.Column.Name));

                ControlData.Conditions = ControlData.Conditions.Where(x => !x.IsEmpty).ToArray();
            }

            Conditions.LoadData();

            SelectSection(ConditionSection);
        }

        private void Conditions_ViewChanged(object sender, BuildViewChangedArgs args)
        {
            var isEdit = args.View == BuildViewType.Edit;

            AddConditionButton.Visible = !isEdit;
            NextButtonCondition.Visible = !isEdit;

            SaveConditionButton.Visible = isEdit;
            CancelSaveConditionButton.Visible = isEdit;
        }

        private void AddConditionButton_Click(object sender, EventArgs e)
        {
            Conditions.Edit(-1);
        }

        private void SaveConditionButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Conditions.Save();
        }

        private void CancelSaveConditionButton_Click(object sender, EventArgs e)
        {
            Conditions.View();
        }

        private void NextButtonCondition_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!IsDataValid())
                return;

            ControlData.Conditions = Conditions.GetConditions();

            BindResultCondition(ControlData.Conditions);

            try
            {
                OnResultConditionChanged();
                SelectSection(ResultSection);
            }
            catch (SqlException ex)
            {
                Status.AddMessage(AlertType.Error, $"{Translate("Can't load report")}. {ex.Message}");
            }
        }

        private void ResultCondition_ValueChanged(object sender, EventArgs e) => OnResultConditionChanged();

        private void OnResultConditionChanged()
        {
            var reportDefinition = GetReportDefinition();

            SqlText.Text = reportDefinition.GetSelectSql(Organization.OrganizationIdentifier, ResultCondition.ValueAsInt);

            var countSql = reportDefinition.GetCountSql(Organization.OrganizationIdentifier, ResultCondition.ValueAsInt);
            ResultPagination.ItemsCount = CreateDatabase(true).SqlQueryCount(countSql);
            ResultCount.Text = $"{"row".ToQuantity(ResultPagination.ItemsCount, "n0")} {Translate("match your conditions")}.";

            TableHtml.Text = GetTableHtml(reportDefinition, ResultPagination.ItemsSkip, ResultPagination.ItemsTake);

            Report = reportDefinition;

            JsonText.Text = reportDefinition.Serialize();
        }

        private void ResultPagination_PageChanged(object sender, Pagination.PageChangedEventArgs e)
        {
            TableHtml.Text = GetTableHtml(Report, ResultPagination.ItemsSkip, ResultPagination.ItemsTake);
        }

        private void ResultDownloadButton_Click(object sender, CommandEventArgs e)
        {
            byte[] data;
            string ext;

            if (e.CommandName == "default" || e.CommandName == "Excel")
            {
                data = GetXlsx();
                ext = "xlsx";
            }
            else if (e.CommandName == "Csv")
            {
                data = GetCsv();
                ext = "csv";
            }
            else
                return;

            if (data == null)
                return;

            Response.SendFile(ReportTitle.IfNullOrEmpty("Build Report"), ext, data);
        }

        private void SaveReportButton_Click(object sender, EventArgs e)
        {
            if (!IsDataValid())
                return;

            var json = GetReportDefinition().Serialize();

            if (ReportIdentifier.HasValue)
            {
                var report = TReportSearch.Select(ReportIdentifier.Value);

                report.ReportData = json;
                report.Modified = DateTimeOffset.Now;
                report.ModifiedBy = User.UserIdentifier;

                TReportStore.Update(report);

                Status.AddMessage(AlertType.Success, Translate("Report has been saved."));
            }
            else
            {
                CreateReportJson = json;

                HttpResponseHelper.Redirect($"/ui/admin/reports/create?action=build", true);
            }
        }

        #endregion

        #region Methods

        private void SelectSection(AccordionPanel panel)
        {
            var isExec = IsExecute;

            DataSetSection.Visible = !isExec;
            AggregateSection.Visible = false;
            ColumnSection.Visible = false;
            ConditionSection.Visible = false;
            ResultSection.Visible = false;
            SettingsSection.Visible = false;
            SaveReportButton.Visible = false;

            if (DataSetSection == panel)
            {
                DataSetSection.IsSelected = true;
                return;
            }

            AggregateSection.Visible = !isExec && DataSetStatistic.Checked;

            if (AggregateSection == panel)
            {
                AggregateSection.IsSelected = true;
                return;
            }

            ColumnSection.Visible = !isExec;

            if (ColumnSection == panel)
            {
                ColumnSection.IsSelected = true;
                return;
            }

            ConditionSection.Visible = !isExec;

            if (ConditionSection == panel)
            {
                ConditionSection.IsSelected = true;
                return;
            }

            ResultSection.Visible = true;
            SettingsSection.Visible = !isExec;
            SaveReportButton.Visible = !isExec;

            ResultSection.IsSelected = true;
        }

        private byte[] GetXlsx()
        {
            var query = SqlText.Text;

            var data = CreateDatabase(false).SqlQuery(query);

            if (data.Rows.Count > 1048576 || data.Columns.Count > 16384)
            {
                Status.AddMessage(AlertType.Error,
                    Translate("Microsoft Excel is limited to 1,048,576 rows by 16,384 columns. Please select a different download file format, or modify your report so that it does not exceed these limits. For more information, refer to") + " <a href=\"https://support.microsoft.com/en-us/office/excel-specifications-and-limits-1672b34d-7043-467e-8e27-269d656771c3\" target=\"_blank\">https://support.microsoft.com/en-us/office/excel-specifications-and-limits-1672b34d-7043-467e-8e27-269d656771c3</a>");
                return null;
            }

            var boldStyle = new XlsxCellStyle { IsBold = true };

            var sheet = new XlsxWorksheet("Build Report");

            for (var i = 0; i < data.Columns.Count; i++)
            {
                sheet.Columns[i].Width = 40;
                sheet.Cells.Add(new XlsxCell(i, 0)
                {
                    Value = data.Columns[i].ColumnName,
                    Style = boldStyle
                });
            }

            for (var rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
            {
                var row = data.Rows[rowIndex];

                for (var colIndex = 0; colIndex < data.Columns.Count; colIndex++)
                {
                    sheet.Cells.Add(new XlsxCell(colIndex, rowIndex + 1)
                    {
                        Value = row[colIndex]
                    });
                }
            }

            return sheet.GetBytes();
        }

        private byte[] GetCsv()
        {
            var query = SqlText.Text;

            var data = CreateDatabase(false).SqlQuery(query);

            var csv = new StringBuilder();

            for (var i = 0; i < data.Columns.Count; i++)
            {
                csv.Append(data.Columns[i].ColumnName);

                if (i != data.Columns.Count - 1) csv.Append(',');
            }

            csv.AppendLine();

            for (var rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
            {
                var row = data.Rows[rowIndex];

                for (var colIndex = 0; colIndex < data.Columns.Count; colIndex++)
                {
                    csv.AppendFormat("\"{0}\"", row[colIndex].ToString().Replace("\"", "\"\""));

                    if (colIndex != data.Columns.Count - 1) csv.Append(',');
                }

                if (rowIndex != data.Rows.Count - 1) csv.AppendLine();
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private void BindResultCondition(IEnumerable<ReportCondition> conditions)
        {
            ResultCondition.Visible = conditions.Count() > 0;

            ResultCondition.LoadItems(
                conditions.Select((x, i) => new Shift.Common.ListItem
                {
                    Text = x.Name,
                    Value = i.ToString()
                }));
        }

        private ReportDefinition GetReportDefinition()
        {
            var reportDefinition = new ReportDefinition(DataSetSelector.SelectedValue);

            reportDefinition.Aggregates.AddRange(ControlData.Aggregates);
            reportDefinition.Columns.AddRange(ControlData.Columns);
            reportDefinition.Conditions.AddRange(ControlData.Conditions);

            return reportDefinition;
        }

        private string GetTableHtml(ReportDefinition report, int skip, int take)
        {
            var builder = new StringBuilder();

            var query = report.GetSelectSql(Organization.OrganizationIdentifier, ResultCondition.ValueAsInt, skip, take);

            var data = CreateDatabase(true).SqlQuery(query);

            {
                builder.Append($"<thead>");

                foreach (DataColumn column in data.Columns)
                    builder.Append($"<th>{column.ColumnName}</th>");

                builder.Append($"</thead>");
            }

            {
                builder.Append($"<tbody>");

                foreach (DataRow row in data.Rows)
                {
                    builder.Append("<tr>");

                    for (var j = 0; j < data.Columns.Count; j++)
                    {
                        builder.Append($"<td>{row[j]}</td>");
                    }

                    builder.Append("</tr>");
                }

                builder.Append($"</tbody>");
            }

            return builder.ToString();
        }

        private Common.Data.Database CreateDatabase(bool isHtml)
            => new Common.Data.Database(ServiceLocator.AppSettings.Database.ConnectionStrings.Shift, Identity.User.TimeZone, isHtml);

        private bool IsDataValid()
        {
            if (ControlData.DataSet == null)
            {
                Status.AddMessage(AlertType.Error, $"{Translate("You must select a Data Set and click Next button")}.");
                return false;
            }

            if (!ControlData.Columns.Any(x => x.ViewAlias == ControlData.DataSet.View.Alias))
            {
                Status.AddMessage(AlertType.Error, $"{Translate("You must select at least one item from the list of Columns for")} {ControlData.DataSet.View.Title}.");
                return false;
            }

            return true;
        }

        #endregion
    }
}
