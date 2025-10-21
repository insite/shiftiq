using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Cmds.Actions.Reporting.Report;
using InSite.Persistence.Plugin.CMDS;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reports
{
    internal class ActiveUsersExcel
    {
        private readonly string _groupBy;
        private readonly Func<IList<string>, string> _csv;
        private readonly Func<IList<ContactRepository2.Employment>, string> _describe;

        public ActiveUsersExcel(string groupBy, Func<IList<string>, string> csv, Func<IList<ContactRepository2.Employment>, string> describe)
        {
            _groupBy = groupBy;
            _csv = csv;
            _describe = describe;
        }

        private void AddXlsxDataRow(
            XlsxWorksheet table, 
            int rowIndex, 
            string group, 
            ContactRepository2.CompanyEmployee employee)
        {
            var colIndex = 0;

            switch (_groupBy)
            {
                case "Organization":
                    AddCell(employee.Name);
                    AddCell(employee.Email);
                    AddCell(_describe(employee.Employments), wrapText: true);
                    AddCell(employee.Profiles, HorizontalAlignment.Right);
                    AddCell(employee.Status);
                    AddCell(employee.LastAuthenticated.HasValue ? employee.LastAuthenticated.Value.UtcDateTime : (DateTime?)null,
                        format: "MMM dd, yyyy");
                    AddCell(_csv(employee.Roles), wrapText: true);
                    AddCell(employee.OrganizationCount, HorizontalAlignment.Right);
                    break;
                case "OrganizationAndDepartment":
                    AddCell(employee.Name);
                    AddCell(employee.Email);
                    AddCell(employee.Profiles, HorizontalAlignment.Right);
                    AddCell(employee.Status);
                    AddCell(employee.LastAuthenticated.HasValue ? employee.LastAuthenticated.Value.UtcDateTime : (DateTime?)null,
                        format: "MMM dd, yyyy");
                    AddCell(_csv(employee.Roles), wrapText: true);
                    AddCell(employee.OrganizationCount, HorizontalAlignment.Right);
                    break;
                case "Role":
                    AddCell(employee.Name);
                    AddCell(employee.Email);
                    table.Cells.Add(
                        AppendEmploymentsXlsxRichText(
                            new XlsxCellRichText(colIndex++, rowIndex)
                            {
                                Style = new XlsxCellStyle
                                {
                                    WrapText = true
                                }
                            },
                            employee.Employments
                        )
                    );
                    AddCell(employee.Profiles, HorizontalAlignment.Right);
                    AddCell(employee.Status);
                    AddCell(employee.LastAuthenticated.HasValue ? employee.LastAuthenticated.Value.UtcDateTime : (DateTime?)null,
                        format: "MMM dd, yyyy");
                    AddCell(employee.OrganizationCount, HorizontalAlignment.Right);
                    break;
            }

            void AddCell(object value, HorizontalAlignment? align = null, bool? wrapText = null, string format = null)
            {
                table.Cells.Add(new XlsxCell(colIndex++, rowIndex)
                {
                    Value = value,
                    Format = format,
                    Style = new XlsxCellStyle
                    {
                        Align = align,
                        WrapText = wrapText
                    }
                });
            }
        }

        private void AddXlsxHeader(XlsxWorksheet table, int rowIndex)
        {
            var headerStyle = new XlsxCellStyle { IsBold = true };
            var colIndex = 0;

            switch (_groupBy)
            {
                case "Organization":
                    AddCell("Name");
                    AddCell("Email");
                    AddCell("Departments");
                    AddCell("Profiles");
                    AddCell("Status");
                    AddCell("Last Authenticated");
                    AddCell("Roles");
                    AddCell("Organizations");
                    break;
                case "OrganizationAndDepartment":
                    AddCell("Name");
                    AddCell("Email");
                    AddCell("Profiles");
                    AddCell("Status");
                    AddCell("Last Authenticated");
                    AddCell("Roles");
                    AddCell("Organizations");
                    break;
                case "Role":
                    AddCell("Name");
                    AddCell("Email");
                    AddCell("Employments");
                    AddCell("Profiles");
                    AddCell("Status");
                    AddCell("Last Authenticated");
                    AddCell("Organizations");
                    break;
            }

            void AddCell(string value)
            {
                table.Cells.Add(new XlsxCell(colIndex++, rowIndex)
                {
                    Style = headerStyle,
                    Value = value
                });
            }
        }

        private XlsxCellRichText AppendEmploymentsXlsxRichText(XlsxCellRichText cell,
            List<ContactRepository2.Employment> employments)
        {
            if (cell.Items.Count > 0)
                cell.AddText("\r\n");

            cell.AddText(_describe(employments));

            return cell;
        }

        public void ExportSearchResultsToXlsx(IDictionary<string, IList<ContactRepository2.CompanyEmployee>> dict)
        {
            var keys = dict.Keys.OrderBy(k => k).ToArray();
            var xlsxSheet = new XlsxWorksheet("Active Users");

            InitXlsxColumns(xlsxSheet);

            var rowIndex = 0;
            var groupHeaderStyle = new XlsxCellStyle { IsBold = true };

            foreach (var group in keys)
            {
                if (rowIndex > 0)
                    rowIndex++;

                xlsxSheet.Cells.Add(new XlsxCell(0, rowIndex++, xlsxSheet.Columns.Count)
                {
                    Style = groupHeaderStyle,
                    Value = group
                });

                AddXlsxHeader(xlsxSheet, rowIndex++);

                foreach (var employee in dict[group].OrderBy(x => x.Name))
                    AddXlsxDataRow(xlsxSheet, rowIndex++, group, employee);
            }

            ReportXlsxHelper.Export(xlsxSheet);
        }

        private void InitXlsxColumns(XlsxWorksheet table)
        {
            switch (_groupBy)
            {
                case "Organization":
                    table.Columns[0].Width = 30; //Name
                    table.Columns[1].Width = 30; //Email
                    table.Columns[2].Width = 60; //Departments
                    table.Columns[3].Width = 20; //Profiles
                    table.Columns[4].Width = 20; //Status
                    table.Columns[5].Width = 20; //LastAuthenticated
                    table.Columns[6].Width = 60; //Roles
                    table.Columns[7].Width = 20; //Companies
                    break;
                case "OrganizationAndDepartment":
                    table.Columns[0].Width = 30; //Name
                    table.Columns[1].Width = 30; //Email
                    table.Columns[2].Width = 20; //Profiles
                    table.Columns[3].Width = 20; //Status
                    table.Columns[4].Width = 20; //LastAuthenticated
                    table.Columns[5].Width = 60; //Roles
                    table.Columns[6].Width = 20; //Companies
                    break;
                case "Role":
                    table.Columns[0].Width = 30; //Name
                    table.Columns[1].Width = 30; //Email
                    table.Columns[2].Width = 60; //Employments
                    table.Columns[3].Width = 20; //Profiles
                    table.Columns[4].Width = 20; //Status
                    table.Columns[5].Width = 20; //LastAuthenticated
                    table.Columns[6].Width = 20; //Companies
                    break;
            }
        }
    }
}
