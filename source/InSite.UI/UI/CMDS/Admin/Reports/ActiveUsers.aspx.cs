using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Sdk.UI;

using BoundField = System.Web.UI.WebControls.BoundField;
using SystemListItem = System.Web.UI.WebControls.ListItem;

namespace InSite.Cmds.Actions.Reports
{
    public partial class ActiveUsers : AdminBasePage, ICmdsUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MembershipFunction.AutoPostBack = true;
            MembershipFunction.SelectedIndexChanged += (x, y) => BindModelToControls();

            DownloadButton.Click += (x, y) =>
            {
                var excel = new ActiveUsersExcel(ddlGroupBy.Value, CreateCsv, DescribeDepartments);
                excel.ExportSearchResultsToXlsx(CreateGroups());
            };

            ddlGroupBy.AutoPostBack = true;
            ddlGroupBy.ValueChanged += (x, y) => BindModelToControls();

            BindMembershipFunctions(true, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            BindModelToControls();
        }

        private void BindMembershipFunctions(bool showAdmin, bool isOrganizationChecked)
        {
            var org = new SystemListItem("Organization");

            var dept = new SystemListItem("Department");

            org.Selected = isOrganizationChecked;

            dept.Selected = true;

            if (showAdmin)
            {
                var admin = new SystemListItem("Administration");

                MembershipFunction.Items.Add(admin);
            }

            MembershipFunction.Items.Add(org);

            MembershipFunction.Items.Add(dept);
        }

        private void BindModelToControls()
        {
            var dict = CreateGroups();
            var keys = dict.Keys.OrderBy(k => k).ToArray();

            switch (ddlGroupBy.Value)
            {
                case "Organization":
                    foreach (var group in keys)
                    {
                        var company = group;

                        var header = new HtmlGenericControl
                        {
                            TagName = "h2",
                            InnerHtml = group
                        };
                        place.Controls.Add(header);

                        var dt = new DataTable();
                        dt.Columns.Add("User");
                        dt.Columns.Add("Employment");
                        dt.Columns.Add("Profiles", typeof(int));
                        dt.Columns.Add("Status");
                        dt.Columns.Add("LastLogin", typeof(DateTimeOffset));
                        dt.Columns.Add("Roles");
                        dt.Columns.Add("OrganizationCount", typeof(int));

                        var employees = dict[group].OrderBy(ce => ce.Name).ToList();
                        foreach (var employee in employees)
                        {
                            var row = dt.NewRow();

                            row["User"] = $"{employee.Name}<br />{employee.Email}";
                            row["Employment"] = DescribeDepartments(employee.Employments);
                            row["Profiles"] = employee.Profiles;
                            row["Status"] = employee.Status;
                            row["LastLogin"] = employee.LastAuthenticated.HasValue ? (object)employee.LastAuthenticated : DBNull.Value;
                            row["Roles"] = CreateCsv(employee.Roles);
                            row["OrganizationCount"] = employee.OrganizationCount;

                            dt.Rows.Add(row);
                        }

                        var gv = new GridView
                        {
                            AutoGenerateColumns = false,
                            CssClass = "table table-striped"
                        };

                        var nameField = new BoundField { HeaderText = "User", DataField = "User" };
                        nameField.ItemStyle.Wrap = false;
                        nameField.HtmlEncode = false;

                        var lastLoginField = new BoundField { HeaderText = "Last Login", DataField = "LastLogin" };
                        lastLoginField.ItemStyle.Wrap = false;
                        lastLoginField.DataFormatString = "{0:MMM d, yyyy}";

                        gv.Columns.Add(nameField);
                        gv.Columns.Add(new BoundField { HeaderText = "Employment", DataField = "Employment" });
                        gv.Columns.Add(new BoundField { HeaderText = "Profiles", DataField = "Profiles" });
                        gv.Columns.Add(new BoundField { HeaderText = "Status", DataField = "Status" });
                        gv.Columns.Add(lastLoginField);
                        gv.Columns.Add(new BoundField { HeaderText = "Roles", DataField = "Roles" });
                        gv.Columns.Add(new BoundField { HeaderText = "Organizations", DataField = "OrganizationCount" });

                        gv.DataSource = dt;
                        gv.DataBind();

                        place.Controls.Add(gv);
                    }

                    break;
                case "OrganizationAndDepartment":
                    foreach (var group in keys)
                    {
                        var strs = group.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        var company = strs[0];
                        var department = strs[1];

                        var header = new HtmlGenericControl
                        {
                            TagName = "h3",
                            InnerHtml = group
                        };
                        place.Controls.Add(header);

                        var dt = new DataTable();
                        dt.Columns.Add("Name");
                        dt.Columns.Add("Email");
                        dt.Columns.Add("Profiles", typeof(int));
                        dt.Columns.Add("Status");
                        dt.Columns.Add("LastLogin", typeof(DateTimeOffset));
                        dt.Columns.Add("Roles");
                        dt.Columns.Add("OrganizationCount", typeof(int));

                        var employees = dict[group].OrderBy(ce => ce.Name).ToList();
                        foreach (var employee in employees)
                        {
                            var row = dt.NewRow();

                            row["Name"] = employee.Name;
                            row["Email"] = employee.Email;
                            row["Profiles"] = employee.Profiles;
                            row["Status"] = employee.Status;
                            row["LastLogin"] = employee.LastAuthenticated.HasValue ? (object)employee.LastAuthenticated : DBNull.Value;
                            row["Roles"] = CreateCsv(employee.Roles);
                            row["OrganizationCount"] = employee.OrganizationCount;

                            dt.Rows.Add(row);
                        }

                        var gv = new GridView
                        {
                            AutoGenerateColumns = false,
                            CssClass = "table table-striped"
                        };

                        var nameField = new BoundField { HeaderText = "Name", DataField = "Name" };
                        nameField.ItemStyle.Wrap = false;

                        var lastLoginField = new BoundField { HeaderText = "Last Login", DataField = "LastLogin" };
                        lastLoginField.ItemStyle.Wrap = false;
                        lastLoginField.DataFormatString = "{0:MMM d, yyyy}";

                        gv.Columns.Add(nameField);
                        gv.Columns.Add(new BoundField { HeaderText = "Email", DataField = "Email" });
                        gv.Columns.Add(new BoundField { HeaderText = "Profiles", DataField = "Profiles" });
                        gv.Columns.Add(new BoundField { HeaderText = "Status", DataField = "Status" });
                        gv.Columns.Add(lastLoginField);
                        gv.Columns.Add(new BoundField { HeaderText = "Roles", DataField = "Roles" });
                        gv.Columns.Add(new BoundField { HeaderText = "Organizations", DataField = "OrganizationCount" });

                        gv.DataSource = dt;
                        gv.DataBind();

                        place.Controls.Add(gv);
                    }

                    break;
                case "Role":
                    foreach (var group in keys)
                    {
                        var role = group;

                        var header = new HtmlGenericControl
                        {
                            TagName = "h3",
                            InnerHtml = group
                        };
                        place.Controls.Add(header);

                        var dt = new DataTable();
                        dt.Columns.Add("Name");
                        dt.Columns.Add("Email");
                        dt.Columns.Add("Employments");
                        dt.Columns.Add("Profiles", typeof(int));
                        dt.Columns.Add("Status");
                        dt.Columns.Add("LastLogin", typeof(DateTimeOffset));
                        dt.Columns.Add("OrganizationCount", typeof(int));

                        var employees = dict[group].OrderBy(ce => ce.Name).ToList();
                        foreach (var employee in employees)
                        {
                            var row = dt.NewRow();

                            row["Name"] = employee.Name;
                            row["Email"] = employee.Email;
                            row["Employments"] = DescribeEmployments(employee.Employments);
                            row["Profiles"] = employee.Profiles;
                            row["Status"] = employee.Status;
                            row["LastLogin"] = employee.LastAuthenticated.HasValue ? (object)employee.LastAuthenticated : DBNull.Value;
                            row["OrganizationCount"] = employee.OrganizationCount;

                            dt.Rows.Add(row);
                        }

                        var gv = new GridView
                        {
                            AutoGenerateColumns = false,
                            CssClass = "table table-striped"
                        };

                        var nameField = new BoundField { HeaderText = "Name", DataField = "Name" };
                        nameField.ItemStyle.Wrap = false;

                        var lastLoginField = new BoundField { HeaderText = "Last Login", DataField = "LastLogin" };
                        lastLoginField.ItemStyle.Wrap = false;
                        lastLoginField.DataFormatString = "{0:MMM d, yyyy}";

                        gv.Columns.Add(nameField);
                        gv.Columns.Add(new BoundField { HeaderText = "Email", DataField = "Email" });
                        gv.Columns.Add(new BoundField
                        {
                            HeaderText = "Employments",
                            DataField = "Employments",
                            HtmlEncode = false
                        });
                        gv.Columns.Add(new BoundField { HeaderText = "Profiles", DataField = "Profiles" });
                        gv.Columns.Add(new BoundField { HeaderText = "Status", DataField = "Status" });
                        gv.Columns.Add(lastLoginField);
                        gv.Columns.Add(new BoundField { HeaderText = "Organizations", DataField = "OrganizationCount" });

                        gv.DataSource = dt;
                        gv.DataBind();

                        place.Controls.Add(gv);
                    }

                    break;
            }


        }

        private IDictionary<string, IList<ContactRepository2.CompanyEmployee>> CreateGroups()
        {
            var data = CreateDataSource();
            var result = new Dictionary<string, IList<ContactRepository2.CompanyEmployee>>();

            switch (ddlGroupBy.Value)
            {
                case "Organization":
                    foreach (var employee in data)
                        foreach (var empl in employee.Employments)
                        {
                            var groupName = empl.Company;

                            if (!result.ContainsKey(groupName))
                                result.Add(groupName, new List<ContactRepository2.CompanyEmployee> { employee });
                            else if (!result[groupName].Any(ce => ce.Name == employee.Name))
                                result[groupName].Add(employee);
                        }

                    break;

                case "OrganizationAndDepartment":
                    foreach (var employee in data)
                        foreach (var empl in employee.Employments)
                        {
                            var groupName = string.Format("{0} - {1}", empl.Company, empl.Department);

                            if (!result.ContainsKey(groupName))
                                result.Add(groupName, new List<ContactRepository2.CompanyEmployee> { employee });
                            else if (!result[groupName].Any(ce => ce.Name == employee.Name))
                                result[groupName].Add(employee);
                        }

                    break;

                case "Role":
                    foreach (var employee in data)
                        foreach (var role in employee.Roles)
                        {
                            var groupName = role;

                            if (!result.ContainsKey(groupName))
                                result.Add(groupName, new List<ContactRepository2.CompanyEmployee> { employee });
                            else result[groupName].Add(employee);
                        }

                    break;
            }

            return result;
        }

        private IList<ContactRepository2.CompanyEmployee> CreateDataSource()
        {
            var employmentTypes = GetEmploymentTypes();
            var data = ContactRepository2.SelectActiveUsers(Organization.Identifier, employmentTypes, ExcludeGroup.Text);

            foreach (var item in data)
                if (item.LastAuthenticated.HasValue && item.LastAuthenticated.Value.Year == 1)
                    item.LastAuthenticated = null;

            return data.ToList();
        }

        private IEnumerable<string> GetEmploymentTypes()
        {
            var list = new List<string>();

            if (MembershipFunction.Items.FindByValue("Administration").Selected)
                list.Add(MembershipType.Administration.ToLowerInvariant());

            if (MembershipFunction.Items.FindByValue("Organization").Selected)
                list.Add(MembershipType.Organization.ToLowerInvariant());

            if (MembershipFunction.Items.FindByValue("Department").Selected)
                list.Add(MembershipType.Department.ToLowerInvariant());

            return list;
        }

        private string DescribeDepartments(IList<ContactRepository2.Employment> employments)
        {
            var types = new[]
            {
                MembershipType.Department,
                MembershipType.Organization,
                MembershipType.Administration
            };

            var builder = new StringBuilder();

            foreach (var type in types)
            {
                var departments = employments
                    .Where(e => string.Equals(e.EmploymentType, type, StringComparison.OrdinalIgnoreCase))
                    .Select(e => $"{e.Department} [{e.Profiles}]")
                    .Distinct()
                    .OrderBy(e => e)
                    .ToList();

                if (departments.Count > 0) builder.AppendFormat("{0}: {1} ", type, string.Join(", ", departments));
            }

            return builder.ToString();
        }

        private string DescribeEmployments(List<ContactRepository2.Employment> employments)
        {
            var builder = new StringBuilder();

            var companies = employments.Select(e => e.Company).Distinct().OrderBy(c => c);
            foreach (var company in companies)
                builder.AppendFormat("<strong>{0}</strong><br/>{1}<br/>", company, DescribeDepartments(employments));

            if (builder.Length >= 5)
                builder.Remove(builder.Length - 5, 5);

            return builder.ToString();
        }

        private string CreateCsv(IList<string> list)
        {
            return string.Join(", ", list.OrderBy(item => item));
        }
    }
}