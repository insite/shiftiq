using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Cmds.Actions.BulkTool.Assign;
using InSite.Cmds.Actions.Reporting.Report;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Reporting
{
    public partial class PhoneList : AdminBasePage, ICmdsUserControl
    {
        #region Classes

        [Serializable]
        private class SearchParameters
        {
            public string Departments { get; set; }
            public string SubTypes { get; set; }
            public string Roles { get; set; }
            public bool IsApproved { get; set; }
            public Guid OrganizationIdentifier { get; set; }
        }

        #endregion

        #region Properties

        private PersonFinderSecurityInfoWrapper _finderSecurityInfo;
        private PersonFinderSecurityInfoWrapper FinderSecurityInfo =>
            _finderSecurityInfo ?? (_finderSecurityInfo = new PersonFinderSecurityInfoWrapper(ViewState));

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifierValidator.ServerValidate += (s, a) => a.IsValid = GetItemList(Departments.Items).IsNotEmpty();

            DownloadXlsx.Click += DownloadXlsx_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            FinderSecurityInfo.LoadPermissions();

            LoadDepartments();
            LoadRoles();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SelectAllDepartmentsButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", DepartmentsField.ClientID);

            DeselectAllDepartmentsButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", DepartmentsField.ClientID);

            SelectAllRolesButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', true);", RolesField.ClientID);

            DeselectAllRolesButton.OnClientClick =
                string.Format("return setCheckboxes('{0}', false);", RolesField.ClientID);
        }

        #endregion

        #region Event handlers

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var departments = GetItemList(Departments.Items);
            var subTypes = GetItemList(RoleTypeSelector.Items);
            var roles = GetItemList(Roles.Items);
            var dataSource = CmdsReportHelper.SelectPhoneList(departments, subTypes, roles, IsApproved.Checked, Organization.Identifier);

            ReportXlsxHelper.ExportToXlsx("Phone List", dataSource.ToList());
        }

        #endregion

        #region Helper methods

        private void LoadDepartments()
        {
            Departments.Items.Clear();

            var filter = new DepartmentFilter { OrganizationIdentifier = Organization.Identifier };

            if (!FinderSecurityInfo.CanSeeAllCompanies && !Identity.HasAccessToAllCompanies)
                filter.UserIdentifier = User.UserIdentifier;

            var departments = ContactRepository3.SelectDepartments(filter);

            foreach (DataRow row in departments.Rows)
                Departments.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Value = row["DepartmentIdentifier"].ToString(),
                    Text = (string)row["DepartmentName"],
                    Selected = true
                });
        }

        private void LoadRoles()
        {
            Roles.Items.Clear();

            var roles = ContactRepository3.SelectRoles();

            foreach (var info in roles)
                Roles.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Value = info.GroupIdentifier.ToString(),
                    Text = info.GroupName,
                    Selected = true
                });
        }

        private static string GetItemList(ListItemCollection items)
        {
            var text = new StringBuilder();

            foreach (System.Web.UI.WebControls.ListItem item in items)
            {
                if (!item.Selected)
                    continue;

                if (text.Length > 0)
                    text.Append(",");

                text.Append(item.Value);
            }

            if (text.Length == 0)
                foreach (System.Web.UI.WebControls.ListItem item in items)
                {
                    if (text.Length > 0)
                        text.Append(",");

                    text.Append(item.Value);
                }

            return text.ToString();
        }

        #endregion
    }
}