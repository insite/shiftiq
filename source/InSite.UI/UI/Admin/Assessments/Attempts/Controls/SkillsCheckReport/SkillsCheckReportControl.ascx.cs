using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Attempts.Controls.SkillsCheckReport
{
    public partial class SkillsCheckReportControl : UserControl
    {
        public static byte[] GetPdf(Page page, Guid attemptId, Guid managerUserId, Guid organizationId, TimeZoneInfo timeZone)
        {
            var report = (SkillsCheckReportControl)page.LoadControl("~/UI/Admin/Assessments/Attempts/Controls/SkillsCheckReport/SkillsCheckReportControl.ascx");
            if (!report.LoadReport(attemptId, managerUserId, organizationId, timeZone))
                return null;

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    report.RenderControl(htmlWriter);
            }

            var date = DateTimeOffset.Now.FormatDateOnly(timeZone);

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                EnablePrintMediaType = true,
                PageSize = PageSizeType.Letter,
                MarginTop = 5,
                MarginBottom = 15,
                HeaderSpacing = 7,

                FooterTextLeft = "Competency Report",
                FooterTextCenter = date,
                FooterTextRight = "Page [page] of [topage]",
                FooterFontName = "Calibri",
                FooterFontSize = 10,
                FooterSpacing = 8.1f,
            };

            var data = HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);

            return data;
        }

        private bool LoadReport(Guid attemptId, Guid managerUserId, Guid organizationId, TimeZoneInfo timeZone)
        {
            var attempt = SkillsCheckReportData.GetAttempt(attemptId);
            if (attempt == null)
                return false;

            var manager = SkillsCheckReportData.GetManager(managerUserId, organizationId);

            FormTitle.Text = attempt.FormTitle;
            UserName.Text = attempt.UserName;
            Completed.Text = attempt.Completed.FormatDateOnly(timeZone);

            ManagerName.Text = manager.UserName;
            EmployerName.Text = manager.EmployerName;

            Score.Text = attempt.IsPassing
                ? $"<span class='badge bg-success'>{attempt.Score:p0}</span>"
                : $"<span class='badge bg-danger'>{attempt.Score:p0}</span>";

            var organization = CurrentSessionState.Identity.Organization;
            var domain = ServiceLocator.AppSettings.Security.Domain;
            var environment = ServiceLocator.AppSettings.Environment;
            var code = organization.OrganizationCode;

            Logo.Src = UrlHelper.GetAbsoluteUrl(domain, environment, organization.PlatformCustomization.PlatformUrl.Logo, code);

            OccupationRepeater.DataSource = attempt.Occupations;
            OccupationRepeater.ItemDataBound += OccupationRepeater_ItemDataBound;
            OccupationRepeater.DataBind();

            return true;
        }

        private void OccupationRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (SkillsCheckReportData.Occupation)e.Item.DataItem;
            var frameworkRepeater = (Repeater)e.Item.FindControl("FrameworkRepeater");
            frameworkRepeater.DataSource = item.Frameworks;
            frameworkRepeater.ItemDataBound += FrameworkRepeater_ItemDataBound;
            frameworkRepeater.DataBind();
        }

        private void FrameworkRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (SkillsCheckReportData.Framework)e.Item.DataItem;
            var areaRepeater = (Repeater)e.Item.FindControl("AreaRepeater");
            areaRepeater.DataSource = item.Areas;
            areaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            areaRepeater.DataBind();
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (SkillsCheckReportData.Area)e.Item.DataItem;
            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = item.Competencies;
            competencyRepeater.DataBind();
        }

        protected string GetStatus()
        {
            var status = (SkillsCheckReportData.StandardStatus)DataBinder.Eval(Page.GetDataItem(), "Status");

            switch (status)
            {
                case SkillsCheckReportData.StandardStatus.Satisfied:
                    return "<span class='badge bg-success'>Satisfied</span>";
                case SkillsCheckReportData.StandardStatus.NotSatisfied:
                    return "<span class='badge bg-danger'>Not Satisfied</span>";
                default:
                    return "<span class='badge bg-warning'>Partially Satisfied</span>";
            }
        }
    }
}