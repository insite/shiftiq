using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Colors;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Reports
{
    public partial class DepartmentUserStatus : SearchPage<TUserStatusFilter>
    {
        #region Properties

        public override string EntityName => "Department User Status";

        private List<DateTimeOffset> SnapshotRepeaterKeys
        {
            get => (List<DateTimeOffset>)ViewState[nameof(SnapshotRepeaterKeys)];
            set => ViewState[nameof(SnapshotRepeaterKeys)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // The default timeout is 110 seconds. This report can take a long time to run so we are allowing 6 minutes.
            Page.Server.ScriptTimeout = 6 * 60;

            SearchResults.Zoom += SearchResults_Zoom;

            SnapshotButton.Click += SnapshotButton_Click;

            SnapshotRepeater.DataBinding += SnapshotRepeater_DataBinding;
            SnapshotRepeater.ItemDataBound += SnapshotRepeater_ItemDataBound;
            SnapshotRepeater.ItemCommand += SnapshotRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            SnapshotTab.Visible = Identity.IsInRole(CmdsRole.Programmers)
                || Identity.IsInRole(CmdsRole.Testers);

            if (!BindSnapshotRepeater())
                return;

            BindMostRecentCalculation();
            BindDepartments(null);
            BindChart(null);
        }

        #endregion

        #region Load searched results

        protected override void LoadSearchedResults()
        {
            base.LoadSearchedResults();

            BindDepartments(SearchResults.Filter);
            BindChart(SearchResults.Filter);
        }

        #endregion

        #region Event handlers

        protected override void OnSearching(TUserStatusFilter filter)
        {
            base.OnSearching(filter);

            DepartmentsPanel.IsSelected = true;

            ZoomPanel.Visible = false;

            BindDepartments(filter);
            BindChart(filter);

            if (!filter.Enabled)
                ScreenStatus.AddMessage(AlertType.Error, "Please select a specific snapshot (or a valid date range) in your report criteria.");
        }

        protected override void OnClearing(SearchCriteriaController<TUserStatusFilter> criteria)
        {
            base.OnClearing(criteria);

            ZoomPanel.Visible = false;

            BindDepartments(null);
            BindChart(null);
        }

        private void SearchResults_Zoom(Guid department, Guid user, string statisticName, Guid organization)
        {
            ZoomPanel.Visible = false;

            var statistic = TUserStatusSearch.SelectFirst(x => x.DepartmentIdentifier == department && x.UserIdentifier == user && x.ItemName == statisticName && x.OrganizationIdentifier == organization);
            if (statistic == null)
                return;

            switch (statistic.ListDomain)
            {
                case "Achievement":
                    ZoomAchievementGrid.LoadData(statistic, SearchResults.Filter?.ShowColumns);

                    ZoomAchievementGrid.Visible = true;
                    ZoomStandardGrid.Visible = false;

                    break;
                case "Standard":
                    ZoomStandardGrid.LoadData(statistic, SearchResults.Filter?.ShowColumns);

                    ZoomAchievementGrid.Visible = false;
                    ZoomStandardGrid.Visible = true;

                    break;
            }

            var statisticDisplayName = AchievementTypes.Pluralize(statistic.ItemName, Organization.Code);

            ZoomAsAt.Text = DateTimeOffset.UtcNow.Format(User.TimeZone, true);
            ZoomDepartmentName.Text = statistic.DepartmentName;
            ZoomUserName.Text = statistic.UserName;
            ZoomStatisticName.Text = $"{HttpUtility.HtmlEncode(statisticDisplayName)} <span class='form-text'>({statistic.ItemName})</span>";

            ZoomPanel.Visible = true;
            ZoomPanel.IsSelected = true;
        }

        private void SnapshotButton_Click(object sender, EventArgs e)
        {
            TUserStatusStore.CreateSnapshot(SearchCriteria.Filter.OrganizationIdentifier);

            ScreenStatus.AddMessage(AlertType.Success, $"Compliance statistics for {Organization.CompanyName} have been recalculated.");

            if (!BindSnapshotRepeater())
                return;

            BindMostRecentCalculation();
            SearchCriteria.LoadSnapshotSelector();
            SearchCriteria.Clear();
            OnSearching(SearchCriteria.Filter);
            SnapshotTab.IsSelected = true;
        }

        private void SnapshotRepeater_DataBinding(object sender, EventArgs e)
        {
            SnapshotRepeaterKeys = new List<DateTimeOffset>();
        }

        private void SnapshotRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (TUserStatusSearch.OrganizationSnapshotInfo)e.Item.DataItem;
                SnapshotRepeaterKeys.Add(dataItem.AsAt);
            }
        }

        private void SnapshotRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var asAt = SnapshotRepeaterKeys[e.Item.ItemIndex];
                TUserStatusStore.Delete(Organization.Key, asAt);

                if (!BindSnapshotRepeater())
                    return;

                SearchCriteria.LoadSnapshotSelector();
                SearchCriteria.Clear();
                OnSearching(SearchCriteria.Filter);
                SnapshotTab.IsSelected = true;
            }
        }

        #endregion

        #region Methods (data binding)

        private void BindMostRecentCalculation()
        {
            var reader = new TUserStatusSearch();

            reader.GetAsAt(Organization.Identifier, out DateTimeOffset? from, out DateTimeOffset? thru);

            if (from.HasValue && thru.HasValue)
            {
                var text = $@"Compliance statistics for {Organization.CompanyName} were last calculated {thru.Humanize()}.";

                // If the variance between the minimum and maximum "As At" values exceeds 60 seconds then display both values.
                if ((thru - from).Value.TotalSeconds > 60)
                    text += $@" ({from.Value.Format(User.TimeZone, true)} to {thru.Value.Format(User.TimeZone, true)}).";
                else
                    text += $@" ({from.Value.Format(User.TimeZone, true)}).";

                SnapshotAlert.AddMessage(AlertType.Information, text);
            }
            else
            {
                SnapshotAlert.Visible = false;
                ScreenStatus.AddMessage(AlertType.Warning, "<strong>No Report Data:</strong>There is no data available for the report at this time.");
            }
        }

        private bool BindSnapshotRepeater()
        {
            var reader = new TUserStatusSearch();
            var data = reader.GetOrganizationSnapshots(Organization.OrganizationIdentifier);
            var hasData = data.Any();

            SnapshotRepeater.Visible = hasData;
            SearchCriteriaTab.Visible = hasData;
            SearchResultsTab.Visible = hasData;

            SnapshotRepeater.DataSource = data;
            SnapshotRepeater.DataBind();

            return hasData;
        }

        private void BindChart(TUserStatusFilter filter)
        {
            Chart.Data.Clear();

            IEnumerable<TUserStatusSearch.ChartDataItem> data = null;

            if (filter != null)
            {
                data = new TUserStatusSearch().GetChartData(filter.Clone());
            }

            var hasData = data != null && data.Any();

            ChartsPanel.Visible = hasData;

            if (!hasData)
                return;

            var datasets = GetStatisticDatasets(data);

            foreach (var item in data)
            {
                var dataset = datasets[item.StatisticName];
                var userDate = TimeZoneInfo.ConvertTime(item.AsAt, User.TimeZone);

                dataset.NewItem(userDate.DateTime, (double)item.ComplianceScore);
            }

            Dictionary<string, DateTimeChartDataset> GetStatisticDatasets(IEnumerable<TUserStatusSearch.ChartDataItem> items)
            {
                var achievementDisplayMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code);
                var names = items.Select(x => x.StatisticName).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                var palette = Palette.GenerateColorPalette(names.Length);
                var result = new Dictionary<string, DateTimeChartDataset>(StringComparer.OrdinalIgnoreCase);

                for (var i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    var color = palette[i];

                    var dataset = (DateTimeChartDataset)Chart.Data.CreateDataset(i.ToString());
                    dataset.Label = achievementDisplayMapping.GetOrDefault(name, name);
                    dataset.BackgroundColor = color;
                    dataset.BorderColor = color;
                    dataset.BorderWidth = 2;
                    dataset.Fill = false;
                    dataset.LineTension = 0.05M;
                    dataset.PointRadius = 1;

                    result.Add(names[i], dataset);
                }

                return result;
            }
        }

        private void BindDepartments(TUserStatusFilter filter)
        {
            DepartmentsPanel.Visible = true;

            if (filter == null)
            {
                DepartmentRepeater.Clear();
                DepartmentsPanel.Visible = false;
            }
            else
            {
                DepartmentsPanel.Visible = DepartmentRepeater.LoadData(filter);
            }
        }

        #endregion

        #region Methods (export)

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("AsAt", "As At", "MMM dd, yyyy", 15),
                new DownloadColumn("OrganizationName", "Organization", null, 20),
                new DownloadColumn("DepartmentName", "Department", null, 30),
                new DownloadColumn("UserName", "User", null, 20),
                new DownloadColumn("DisplayItemName", "Statistic", null, 40),
                new DownloadColumn("CountCP", "CP", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountEX", "EX", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountNC", "NC", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountNA", "NA", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountNT", "NT", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountSA", "SA", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountSV", "SV", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountVA", "VA", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("CountRQ", "RQ", null, 5, HorizontalAlignment.Right),
                new DownloadColumn("Score", "Score", "0%", 10, HorizontalAlignment.Right)
            };
        }

        #endregion
    }
}