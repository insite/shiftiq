using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using DocumentFormat.OpenXml.Spreadsheet;

using Humanizer;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reports
{
    public partial class CompetencyListingPerDepartment : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class ReportParameters
        {
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid? ProfileIdentifier { get; set; }
            public bool? IsTimeSensitive { get; set; }
            public string Priority { get; set; }
            public bool ShowExcludedCompetencies { get; set; }
        }

        private class ExcelGroup
        {
            public ExcelGroup(string heading)
            {
                Heading = heading;
                Items = new List<ExcelItem>();
            }

            public string Heading { get; }
            public List<ExcelItem> Items { get; }
        }

        private class ExcelItem
        {
            public string CompetencyNumber { get; set; }
            public string CompetencySummary { get; set; }
            public string Profile { get; set; }
            public string CompetencyTimeSensitivity { get; set; }
            public string CompetencyCriticality { get; set; }
            public string CompetencyLifetime { get; set; }
        }

        private class ProfileDto
        {
            public ProfileDto(string number, string title, bool isCritical, bool isTimeSensitive, string lifetime)
            {
                ProfileNumber = number;
                ProfileTitle = title;
                CompetencyCriticality = isCritical ? "Critical" : "Non-Critical";
                CompetencyTimeSensitivity = isTimeSensitive ? "Time-Sensitive" : "";
                CompetencyLifetime = lifetime;
            }

            public string ProfileNumber { get; }
            public string ProfileTitle { get; }

            public string CompetencyTimeSensitivity { get; }
            public string CompetencyCriticality { get; }

            public bool CompetencyIsCritical => CompetencyCriticality == "Critical";
            public bool CompetencyIsTimeSensitive => CompetencyTimeSensitivity == "Time-Sensitive";

            public string CompetencyLifetime { get; set; }
            public string CompetencyLifetimeHtml
            {
                get
                {
                    if (CompetencyIsTimeSensitive && CompetencyLifetime.HasValue())
                        return $"<span class='badge bg-custom-default'><i class='far fa-check'></i> {CompetencyLifetime}</span>";
                    return string.Empty;
                }
            }
        }

        private class CompetencyDto
        {
            public CompetencyDto()
            {
                Profiles = new List<ProfileDto>();
            }

            public string CompetencyNumber { get; set; }
            public string CompetencySummary { get; set; }

            public List<ProfileDto> Profiles { get; set; }
        }

        private class DepartmentDto
        {
            public DepartmentDto()
            {
                Competencies = new List<CompetencyDto>();
            }

            public string DepartmentName { get; set; }
            public List<CompetencyDto> Competencies { get; set; }

            public int CountCompetencies()
            {
                return Competencies.Count;
            }

            public int CountProfiles()
            {
                var dictionary = new Dictionary<string, int>();

                foreach (var competency in Competencies)
                    foreach (var profile in competency.Profiles)
                    {
                        if (string.IsNullOrEmpty(profile.ProfileNumber))
                            continue;

                        if (!dictionary.ContainsKey(profile.ProfileNumber))
                            dictionary[profile.ProfileNumber] = 0;

                        dictionary[profile.ProfileNumber]++;
                    }

                return dictionary.Keys.Count;
            }

            public ExcelGroup Flatten()
            {
                var group = new ExcelGroup(DepartmentName);

                foreach (var competency in Competencies)
                {
                    if (competency.Profiles.Count == 0)
                    {
                        group.Items.Add(new ExcelItem
                        {
                            CompetencyNumber = competency.CompetencyNumber,
                            CompetencySummary = competency.CompetencySummary
                        });
                    }
                    else
                    {
                        foreach (var profile in competency.Profiles)
                        {
                            group.Items.Add(new ExcelItem
                            {
                                CompetencyNumber = competency.CompetencyNumber,
                                CompetencySummary = competency.CompetencySummary,
                                Profile = profile.ProfileNumber + ": " + profile.ProfileTitle,
                                CompetencyCriticality = profile.CompetencyCriticality,
                                CompetencyTimeSensitivity = profile.CompetencyTimeSensitivity,
                                CompetencyLifetime = profile.CompetencyLifetime
                            });
                        }
                    }
                }

                return group;
            }
        }

        #endregion

        #region Properties

        private ReportParameters Parameters
        {
            get => (ReportParameters)ViewState[nameof(Parameters)];
            set => ViewState[nameof(Parameters)] = value;
        }

        #endregion

        #region Methods (initialization)

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Department.AutoPostBack = true;
            Department.ValueChanged += Department_ValueChanged;

            ReportButton.Click += ReportButton_Click;

            ReportList.ItemDataBound += ReportList_ItemDataBound;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            InitSelectorsByCompany();
        }

        private void InitSelectorsByCompany()
        {
            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                Department.Filter.UserIdentifier = User.UserIdentifier;

            Department.Value = null;

            DepartmentCompetenciesField.Visible = false;

            CurrentProfile.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
            CurrentProfile.Filter.DepartmentIdentifier = null;
            CurrentProfile.Value = null;
        }

        #endregion

        #region Methods (event handling)

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (Parameters == null)
                return;

            var dataSource = CreateDataSource(Parameters);
            var group = dataSource.Flatten();

            if (group.Items.IsEmpty())
            {
                ScreenStatus.AddMessage(AlertType.Error, "Empty data source.");
                return;
            }

            var helper = new XlsxExportHelper();
            helper.Map("Profile", "Profile", null, 40, HorizontalAlignment.Left);
            helper.Map("CompetencyNumber", "Number", null, 20, HorizontalAlignment.Left);
            helper.Map("CompetencySummary", "Summary", null, 50, HorizontalAlignment.Left);
            helper.Map("CompetencyCriticality", "Criticality", null, 20, HorizontalAlignment.Left);
            helper.Map("CompetencyTimeSensitivity", "Time-Sensitivity", null, 20, HorizontalAlignment.Left);
            helper.Map("CompetencyLifetime", "Lifetime", null, 20, HorizontalAlignment.Left);

            var bytes = helper.GetXlsxBytes(excel =>
            {
                var sheet = excel.Workbook.Worksheets.Add(Route.Title);
                sheet.Cells.Style.WrapText = true;

                var groupCells = sheet.Cells[1, 1, 1, 6];
                groupCells.Merge = true;
                groupCells.Value = group.Heading;
                groupCells.StyleName = XlsxExportHelper.HeaderStyleName;

                helper.InsertHeader(sheet, 2, 1);
                helper.InsertData(sheet, group.Items, 3, 1);
                helper.ApplyColumnWidth(sheet, 1, true);
            });

            ReportXlsxHelper.ExportToXlsx(Route.Title, bytes);
        }

        private void Department_ValueChanged(object sender, EventArgs e)
        {
            DepartmentCompetenciesField.Visible = Department.HasValue;

            CurrentProfile.Filter.DepartmentIdentifier = Department.Value;
            CurrentProfile.Value = null;
        }

        private void ReportList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType != ListViewItemType.DataItem)
                return;

            var dto = (CompetencyDto)e.Item.DataItem;
            var list = (ListView)e.Item.FindControl("ProfileList");
            list.DataSource = dto.Profiles;
            list.DataBind();
        }

        #endregion

        #region Methods (data binding)

        private void LoadReport()
        {
            var department = Department.Item;

            Parameters = new ReportParameters
            {
                DepartmentIdentifier = department.Value,
                DepartmentName = department.Text,
                ProfileIdentifier = CurrentProfile.Value,
                IsTimeSensitive = TimeSensitive.SelectedValue.IsEmpty() ? (bool?)null : bool.Parse(TimeSensitive.SelectedValue),
                Priority = Priority.Value.NullIfEmpty(),
                ShowExcludedCompetencies = bool.Parse(DepartmentCompetencies.SelectedValue)
            };

            var data = CreateDataSource(Parameters);

            if (data.Competencies.IsNotEmpty())
            {
                ReportTitle.InnerHtml = data.DepartmentName;
                ReportSubtitle.InnerHtml = "competency".ToQuantity(data.CountCompetencies(), "n0")
                                           + " in "
                                           + "profile".ToQuantity(data.CountProfiles());

                ReportList.DataSource = data.Competencies;
                ReportList.DataBind();

                ReportTab.Visible = true;
                ReportTab.IsSelected = true;
            }
            else
            {
                ReportTab.Visible = false;
                CriteriaTab.IsSelected = true;

                ScreenStatus.AddMessage(AlertType.Information, "No results found matching your search criteria.");
            }
        }

        private static DepartmentDto CreateDataSource(ReportParameters parameters)
        {
            var data = CmdsReportHelper.SelectDepartmentCompetencies(
                parameters.DepartmentIdentifier,
                parameters.ProfileIdentifier,
                parameters.IsTimeSensitive,
                parameters.Priority,
                false,
                parameters.ShowExcludedCompetencies
            );

            var dto = new DepartmentDto
            {
                DepartmentName = parameters.DepartmentName,
                Competencies = data
                    .OrderBy(y => y.Number)
                    .GroupBy(y => new { y.Number, y.Summary })
                    .Select(z => new CompetencyDto
                    {
                        CompetencyNumber = z.Key.Number,
                        CompetencySummary = z.Key.Summary,

                        Profiles = z.Select(a => new ProfileDto(
                            a.ProfileNumber,
                            a.ProfileTitle,
                            a.PriorityText == "Critical",
                            a.IsTimeSensitive,
                            a.ValidForText
                        )).ToList()
                    })
                    .ToList()
            };

            return dto;
        }

        #endregion
    }
}
