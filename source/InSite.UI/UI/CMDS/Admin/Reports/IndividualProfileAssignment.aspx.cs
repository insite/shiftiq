using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class IndividualProfileAssignment : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string RoleType { get; set; }
        }

        private class GroupItem
        {
            public string GroupName { get; set; }
            public IEnumerable<CmdsReportHelper.IndividualProfileAssignment> List { get; set; }
        }

        #endregion

        #region Properties

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private PersonFinderSecurityInfoWrapper FinderSecurityInfo => _finderSecurityInfo
            ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        private SearchParameters CurrentParameters
        {
            get => (SearchParameters)ViewState[nameof(CurrentParameters)];
            set => ViewState[nameof(CurrentParameters)] = value;
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportButton.Click += ReportButton_Click;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            FinderSecurityInfo.LoadPermissions();

            InitSelectors();
        }

        private void InitSelectors()
        {
            Department.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            if (!Identity.HasAccessToAllCompanies)
                Department.Filter.UserIdentifier = User.UserIdentifier;

            Department.Value = null;
        }

        #endregion

        #region Event handlers

        private void ReportButton_Click(object sender, EventArgs e)
        {
            ReportTab.Visible = false;

            if (Page.IsValid)
                LoadReport();
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var fileName = $"{Route.Title} - {CompanyName.Text} ({DepartmentName.Text})";

            if (CurrentParameters == null)
                return;

            var dataSource = CmdsReportHelper.SelectIndividualProfileAssignment(CurrentParameters.DepartmentIdentifier, CurrentParameters.RoleType)
                .OrderBy(x => x.PersonName)
                .Select(x => new CmdsReportHelper.IndividualProfileAssignment
                {
                    PersonName = x.PersonName,
                    PrimaryProfileName = x.PrimaryProfileName,
                    SecondaryRequiredProfiles = x.SecondaryRequiredProfiles?.Replace("• ", ""),
                    SecondaryProfiles = x.SecondaryProfiles?.Replace("• ", ""),
                    Managers = x.Managers?.Replace("• ", ""),
                    Supervisors = x.Supervisors?.Replace("• ", ""),
                    Validators = x.Validators?.Replace("• ", "")
                })
                .ToArray();

            var helper = new XlsxExportHelper();
            helper.Map("PersonName", "Employee Name", null, 26, HorizontalAlignment.Left);
            helper.Map("PrimaryProfileName", "Primary Profile", null, 24, HorizontalAlignment.Left);
            helper.Map("SecondaryRequiredProfiles", "Secondary Required for Compliance Profile(s)", null, 30, HorizontalAlignment.Left);
            helper.Map("SecondaryProfiles", "Secondary Not Required for Compliance Profile(s)", null, 30, HorizontalAlignment.Left);
            helper.Map("Managers", "Manager(s)", null, 30, HorizontalAlignment.Left);
            helper.Map("Supervisors", "Supervisor(s)", null, 30, HorizontalAlignment.Left);
            helper.Map("Validators", "Validator(s)", null, 30, HorizontalAlignment.Left);

            var bytes = helper.GetXlsxBytes(excel =>
            {
                var sheet = excel.Workbook.Worksheets.Add(Route.Title);
                sheet.Cells.Style.WrapText = true;

                var groupCells = sheet.Cells[1, 1, 1, 7];
                groupCells.Merge = true;
                groupCells.Value = $"{CompanyName.Text} :: {DepartmentName.Text}";
                groupCells.StyleName = XlsxExportHelper.HeaderStyleName;

                helper.InsertHeader(sheet, 2, 1);
                helper.InsertData(sheet, dataSource, 3, 1);
                helper.ApplyColumnWidth(sheet, 1, true);
            });

            ReportXlsxHelper.ExportToXlsx(Route.Title, bytes, fileName);
        }

        #endregion

        #region Data binding

        private void LoadReport()
        {
            CurrentParameters = new SearchParameters
            {
                OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier,
                DepartmentIdentifier = Department.Value.Value,
                RoleType = GetRoleType()
            };

            var dataSource = CmdsReportHelper.SelectIndividualProfileAssignment(CurrentParameters.DepartmentIdentifier, CurrentParameters.RoleType)
                .OrderBy(x => x.PersonName);

            foreach (var item in dataSource)
            {
                item.SecondaryRequiredProfiles = item.SecondaryRequiredProfiles?.Replace("\r\n", "<br />");
                item.SecondaryProfiles = item.SecondaryProfiles?.Replace("\r\n", "<br />");
                item.Managers = item.Managers?.Replace("\r\n", "<br />");
                item.Supervisors = item.Supervisors?.Replace("\r\n", "<br />");
                item.Validators = item.Validators?.Replace("\r\n", "<br />");
            }

            ReportTab.Visible = true;
            ReportTab.IsSelected = true;

            DownloadButton.Visible = dataSource.Any();

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();

            CompanyName.Text = OrganizationSearch.Select(CurrentIdentityFactory.ActiveOrganizationIdentifier).Name;
            DepartmentName.Text = DepartmentSearch.Select(Department.Value.Value).DepartmentName;
        }

        #endregion

        #region Helper methods

        private string GetRoleType()
        {
            var list = new StringBuilder();

            foreach (ListItem item in RoleTypeSelector.Items)
            {
                if (!item.Selected)
                    continue;

                if (list.Length > 0)
                    list.Append(",");

                list.Append(item.Value);
            }

            if (list.Length == 0)
                foreach (ListItem item in RoleTypeSelector.Items)
                {
                    if (list.Length > 0)
                        list.Append(",");

                    list.Append(item.Value);
                }

            return list.ToString();
        }

        #endregion
    }
}