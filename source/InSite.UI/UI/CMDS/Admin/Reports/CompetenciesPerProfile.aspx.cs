using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Cmds.Actions.Reporting.Report
{
    public partial class CompetenciesPerProfile : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        private const string SearchUrl = "/ui/cmds/admin/standards/profiles/search";

        private Guid? ProfileStandardIdentifier => Guid.TryParse(Request["id"], out Guid result) ? result : (Guid?)null;

        private bool IsOutlineParent => Request.QueryString["parent"] == "outline";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadXlsx.Click += (s, a) => SendXlsx();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadReport();
        }

        private void LoadReport()
        {
            var profileInfo = ProfileStandardIdentifier.HasValue
                ? StandardSearch.Select(ProfileStandardIdentifier.Value)
                : null;

            if (profileInfo == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"Profile #{profileInfo.Code} - {profileInfo.ContentTitle}");

            var dataSource = CmdsReportHelper.SelectCompetencyPerProfile(ProfileStandardIdentifier ?? Guid.Empty);

            DownloadCommandsPanel.Visible = dataSource.Any();

            DataRepeater.DataSource = dataSource;
            DataRepeater.DataBind();

            CloseButton.NavigateUrl = GetReturnUrl() + $"?id={ProfileStandardIdentifier}";
        }

        private void SendXlsx()
        {
            var profileInfo = ProfileStandardIdentifier.HasValue
                ? StandardSearch.Select(ProfileStandardIdentifier.Value)
                : null;

            if (profileInfo == null)
                return;

            var dataSource = CmdsReportHelper.SelectCompetencyPerProfile(ProfileStandardIdentifier ?? Guid.Empty);
            if (!dataSource.Any())
                return;

            var title = string.IsNullOrEmpty(profileInfo.Code) ? "Profile" : $"Profile {profileInfo.Code}";

            var xlsxSheet = new XlsxWorksheet(title);
            xlsxSheet.WrapText = true;
            xlsxSheet.Columns[0].Width = 15;
            xlsxSheet.Columns[1].Width = 45;
            xlsxSheet.Columns[2].Width = 45;
            xlsxSheet.Columns[3].Width = 45;
            xlsxSheet.Columns[4].Width = 10;

            var headerCellStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            var itemCellStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            var hourCellStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var hourCellFormat = "#,##0.00";

            xlsxSheet.Cells.Add(new XlsxCell(0, 0) { Value = profileInfo.Code, Style = itemCellStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, 0) { Value = profileInfo.ContentTitle, Style = itemCellStyle });

            xlsxSheet.Cells.Add(new XlsxCell(0, 1) { Value = "Competency #", Style = headerCellStyle });
            xlsxSheet.Cells.Add(new XlsxCell(1, 1) { Value = "Competency Summary", Style = headerCellStyle });
            xlsxSheet.Cells.Add(new XlsxCell(2, 1) { Value = "Competency Knowledge", Style = headerCellStyle });
            xlsxSheet.Cells.Add(new XlsxCell(3, 1) { Value = "Competency Skills", Style = headerCellStyle });
            xlsxSheet.Cells.Add(new XlsxCell(4, 1) { Value = "Hours", Style = headerCellStyle });

            var rowNumber = 2;

            foreach (var dataItem in dataSource)
            {
                xlsxSheet.Cells.Add(new XlsxCell(0, rowNumber) { Value = dataItem.CompetencyNumber, Style = itemCellStyle });
                xlsxSheet.Cells.Add(new XlsxCell(1, rowNumber) { Value = dataItem.CompetencySummary, Style = itemCellStyle });
                xlsxSheet.Cells.Add(new XlsxCell(2, rowNumber) { Value = dataItem.CompetencyKnowledge, Style = itemCellStyle });
                xlsxSheet.Cells.Add(new XlsxCell(3, rowNumber) { Value = dataItem.CompetencySkills, Style = itemCellStyle });
                xlsxSheet.Cells.Add(new XlsxCell(4, rowNumber) { Value = dataItem.ProgramHours, Style = hourCellStyle, Format = hourCellFormat });

                rowNumber++;
            }

            ReportXlsxHelper.Export(xlsxSheet);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit") || parent.Name.EndsWith("/outline")
                ? $"id={ProfileStandardIdentifier}"
                : null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent()
            => GetParent();

        protected override string GetReturnUrl()
            => IsOutlineParent ? "/ui/cmds/admin/standards/profiles/outline" : "/ui/cmds/admin/standards/profiles/edit";
    }
}