using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Admin.Contact.Persons
{
    public partial class Anomalies : AdminBasePage
    {
        private class PersonItem
        {
            public string Name { get; set; }
            public string City { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Problem { get; set; }
            public string Solution { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadXlsx.Click += DownloadXlsx_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            LoadReport();
        }

        private void DownloadXlsx_Click(object sender, EventArgs e)
        {
            const string name = "Problem Report";

            var dataSource = GetReportDataSource();

            var helper = new XlsxExportHelper();
            helper.Map("Name", "Name", null, 30, HorizontalAlignment.Left);
            helper.Map("City", "City", null, 30, HorizontalAlignment.Left);
            helper.Map("Phone", "Phone", null, 30, HorizontalAlignment.Left);
            helper.Map("Email", "Email", null, 30, HorizontalAlignment.Left);
            helper.Map("Problem", "Problem", null, 30, HorizontalAlignment.Left);

            var bytes = helper.GetXlsxBytes(excel =>
            {
                var sheet = excel.Workbook.Worksheets.Add(name);
                sheet.Cells.Style.WrapText = true;

                var row = helper.InsertData(sheet, dataSource, 1, 1);

                helper.ApplyColumnWidth(sheet, 1, true);
            });

            ReportXlsxHelper.ExportToXlsx(name, bytes);
        }

        private void LoadReport()
        {
            var data = GetReportDataSource();
            var hasData = data.Count > 0;

            ProblemRepeater.Visible = hasData;
            DownloadCommandsPanel.Visible = hasData;

            ProblemRepeater.DataSource = data;
            ProblemRepeater.DataBind();
        }

        private List<PersonItem> GetReportDataSource()
        {
            var problems = new List<PersonItem>();

            var emailProblems = PersonCriteria.Bind(
                x => new
                {
                    Name = x.User.FullName,
                    City = x.HomeAddress.City ?? x.BillingAddress.City,
                    x.Phone,
                    x.User.Email,
                    Problem = x.User.Email == null ? "Missing Email" : "Invalid Email"
                },
                new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    IsArchived = false,
                    IsEmailPatternValid = false,
                    OrderBy = "Name"
                }
            );

            foreach (var row in emailProblems)
            {
                var person = new PersonItem
                {
                    Name = row.Name,
                    City = row.City,
                    Phone = row.Phone,
                    Email = row.Email,
                    Problem = row.Problem
                };

                problems.Add(person);
            }

            var names = PersonCriteria.Bind(
                x => new
                {
                    x.User.FullName,
                    x.User.FirstName,
                    x.User.MiddleName,
                    x.User.LastName,
                    City = x.HomeAddress.City ?? x.BillingAddress.City,
                    x.Phone,
                    x.User.Email,

                    PersonFullName = x.FullName ?? x.User.FullName,
                    PersonFullNameSuffix = x.EmployeeType,
                    x.Organization.PersonFullNamePolicy
                },
                new PersonFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    IsArchived = false
                });

            names = names.OrderBy(x => x.FullName).ToArray();

            foreach (var name in names)
            {
                var expectedPersonFullName = UserNameHelper.GetFullName(
                    name.PersonFullNamePolicy,
                    name.FirstName,
                    name.MiddleName,
                    name.LastName,
                    name.PersonFullNameSuffix);

                var isExpectedPersonName = StringHelper.Equals(expectedPersonFullName, name.PersonFullName);

                if (!isExpectedPersonName)
                {
                    var person = new PersonItem
                    {
                        Name = name.PersonFullName,
                        City = name.City,
                        Phone = name.Phone,
                        Email = name.Email,
                        Problem = "Unexpected Full Name",
                        Solution = "Rename to " + expectedPersonFullName
                    };

                    problems.Add(person);
                }
            }

            return problems;
        }
    }
}