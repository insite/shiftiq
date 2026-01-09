using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Toolbox;

namespace InSite.Custom.CMDS.Admin.Competencies.Controls
{
    public partial class ValidationGrid : BaseUserControl
    {
        private StandardValidationFilter Filter
        {
            get => (StandardValidationFilter)ViewState[nameof(Filter)];
            set => ViewState[nameof(Filter)] = value;
        }

        public int RowCount => Grid.VirtualItemCount;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ScriptManager.GetCurrent(Page).RegisterPostBackControl(DownloadButton);
        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            var filter = Filter;
            if (filter == null)
                return;

            filter.Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            Grid.DataSource = GetData(filter);
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            var filter = Filter;
            if (filter == null)
                return;

            filter.Paging = null;

            var table = GetData(filter);
            if (table.Columns.Count == 0 || table.Rows.Count == 0)
                return;

            var helper = new XlsxExportHelper();

            foreach (DataColumn column in table.Columns)
            {
                if (column.ColumnName == "LifetimeMonthsHtml")
                    continue;

                var align = XlsxCellStyle.GetAlign(column.DataType);
                var format = XlsxCellStyle.GetFormat(column.DataType);

                helper.Map(column.ColumnName, column.ColumnName, format, 30, align);
            }

            var data = helper.GetXlsxBytes(table.DefaultView, "Validations");
            var filename = string.Format("validations-{0}-{1:yyyyMMdd}-{1:HHmmss}", filter.StandardIdentifier, DateTime.UtcNow);

            Page.Response.SendFile(filename, "xlsx", data);
        }

        public void LoadData(Guid standardKey)
        {
            var filter = Filter = new StandardValidationFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                StandardIdentifier = standardKey
            };

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = StandardValidationSearch.Count(filter);
            Grid.DataBind();

            DownloadButton.Visible = Grid.VirtualItemCount > 0;
        }

        private static DataTable GetData(StandardValidationFilter filter)
        {
            filter.OrderBy = "UserFullName";

            var data = StandardValidationSearch.Bind(x => new
            {
                x.ValidationIdentifier,
                x.StandardIdentifier,
                x.UserIdentifier,
                UserFullName = x.User.FullName,
                x.Expired,
                x.ValidationDate,
                x.ValidatorUserIdentifier,
            }, filter);

            var table = new DataTable();

            table.Columns.Add("ValidationIdentifier", typeof(Guid));
            table.Columns.Add("UserFullName", typeof(string));
            table.Columns.Add("DepartmentName", typeof(string));
            table.Columns.Add("ProfileStandardCode", typeof(string));
            table.Columns.Add("IsCritical", typeof(bool));
            table.Columns.Add("IsTimeSensitive", typeof(bool));
            table.Columns.Add("Expired", typeof(DateTimeOffset));
            table.Columns.Add("ValidationDate", typeof(DateTimeOffset));
            table.Columns.Add("ValidatorFullName", typeof(string));

            foreach (var item in data)
            {
                var row = table.NewRow();

                row["ValidationIdentifier"] = item.ValidationIdentifier;
                row["UserFullName"] = item.UserFullName;
                row["IsCritical"] = false;
                row["IsTimeSensitive"] = false;

                row["Expired"] = (object)item.Expired ?? DBNull.Value;
                row["ValidationDate"] = (object)item.ValidationDate ?? DBNull.Value;
                row["ValidatorFullName"] = item.ValidatorUserIdentifier.HasValue ? (object)UserSearch.Select(item.ValidatorUserIdentifier.Value).FullName : DBNull.Value;

                var departmentProfileUsers = DepartmentProfileUserSearch.Select(x => x.UserIdentifier == item.UserIdentifier);
                var departmentIdentifiers = new List<Guid>();
                var profileStandardIdentifiers = new List<Guid>();
                foreach (var departmentProfileUser in departmentProfileUsers)
                {
                    departmentIdentifiers.Add(departmentProfileUser.DepartmentIdentifier);
                    profileStandardIdentifiers.Add(departmentProfileUser.ProfileStandardIdentifier);
                }

                var departmentProfileCompetency = DepartmentProfileCompetencySearch.SelectFirst(x =>
                    (departmentIdentifiers.Count == 0 || departmentIdentifiers.Contains(x.DepartmentIdentifier)) &&
                    (profileStandardIdentifiers.Count == 0 || profileStandardIdentifiers.Contains(x.ProfileStandardIdentifier)) &&
                    x.CompetencyStandardIdentifier == item.StandardIdentifier, x => x.Department, x => x.Profile);

                if (departmentProfileCompetency != null)
                {
                    row["IsCritical"] = departmentProfileCompetency.IsCritical;
                    row["IsTimeSensitive"] = departmentProfileCompetency.LifetimeMonths != null;

                    row["DepartmentName"] = departmentProfileCompetency.Department != null ? (object)departmentProfileCompetency.Department.DepartmentName : DBNull.Value;
                    row["ProfileStandardCode"] = departmentProfileCompetency.Profile != null ? (object)departmentProfileCompetency.Profile.Code : DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        protected string GetBoolean(string name)
        {
            var dataItem = Page.GetDataItem();
            var value = (bool)DataBinder.Eval(dataItem, name);

            return value ? "Yes" : "No";
        }

        protected string GetLocalTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var value = DataBinder.Eval(dataItem, name) as DateTimeOffset?;

            return value.Format(User.TimeZone, true);
        }
    }
}