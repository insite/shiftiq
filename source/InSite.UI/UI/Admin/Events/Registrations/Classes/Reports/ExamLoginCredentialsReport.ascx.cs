using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Registrations.Read;
using InSite.Common.Web;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Registrations.Reports
{
    public partial class ExamLoginCredentialsReport : UserControl
    {
        internal class RegistrationItem
        {
            public int Number { get; set; }
            public string UserFullName { get; set; }
            public string PersonCode { get; set; }
            public string UserEmail { get; set; }
            public string Password { get; set; }
            public string ExamLoginUrl { get; set; }

            public RegistrationItem(QRegistration registration)
            {
                Number = registration.RegistrationSequence ?? 0;
                Password = registration.RegistrationPassword;

                var candidate = registration.Candidate;
                if (candidate == null)
                    return;

                UserFullName = candidate.UserFullName;
                PersonCode = candidate.PersonCode;
                UserEmail = candidate.UserEmail;
            }
        }

        internal class EventInfo
        {
            public string EventTitle { get; set; }
            public string EventStart { get; set; }
            public string EventEnd { get; set; }
            public string EventClassCode { get; set; }
            public string ExamLoginUrl { get; set; }

            public List<RegistrationItem> Registrations { get; set; }
        }

        private const string ReportTitle = "Exam Login Credentials";

        internal static byte[] GetPdf(EventInfo @event)
        {
            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (ExamLoginCredentialsReport)page.LoadControl("~/UI/Admin/Events/Registrations/Classes/Reports/ExamLoginCredentialsReport.ascx");

                report.PageTitle.InnerText = ReportTitle;
                report.TotalRegistrations.Text = @event.Registrations.Count.ToString();

                report.RegistrationRepeater.DataSource = @event.Registrations;
                report.RegistrationRepeater.DataBind();

                var siteContent = new StringBuilder();
                using (var stringWriter = new StringWriter(siteContent))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                        report.RenderControl(htmlWriter);
                }

                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
                {
                    PageOrientation = PageOrientationType.Landscape,
                    Viewport = new HtmlConverterSettings.ViewportSize(1400, 980),
                    MarginTop = 24,
                    MarginBottom = 22,
                    Dpi = 240,

                    HeaderUrl = HttpRequestHelper.GetAbsoluteUrl("~/UI/Admin/Events/Registrations/Classes/Reports/ExamLoginCredentialsReportHeader.html"),
                    HeaderSpacing = 5f,
                    Variables = new HtmlConverterSettings.Variable[]
                    {
                    new HtmlConverterSettings.Variable("title", ReportTitle),
                    new HtmlConverterSettings.Variable("subtitle", $"{@event.EventTitle} #{@event.EventClassCode} ({@event.EventStart} - {@event.EventEnd})"),
                    },

                    FooterTextLeft = ReportTitle,
                    FooterTextCenter = DateTimeOffset.Now.FormatDateOnly(CurrentSessionState.Identity.User.TimeZone),
                    FooterTextRight = "Page [page] of [topage]",
                    FooterFontName = "Calibri",
                    FooterFontSize = 10,
                    FooterSpacing = 8.1f,
                };

                return HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);
            }
        }

        internal static byte[] GetXlsx(EventInfo @event)
        {
            var regsCount = @event.Registrations.Count;

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var normalCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };

            var sheet = new XlsxWorksheet(ReportTitle);

            sheet.Columns[0].Width = 5;
            sheet.Columns[1].Width = 30;
            sheet.Columns[2].Width = 15;
            sheet.Columns[3].Width = 35;
            sheet.Columns[4].Width = 20;
            sheet.Columns[5].Width = 45;

            var title = $"Exam Login Credentials \n" +
                $"{@event.EventTitle} #{@event.EventClassCode} ({@event.EventStart} - {@event.EventEnd})";

            sheet.Rows[0].Height = 30;
            sheet.Cells.Add(new XlsxCell(0, 0, 6) { Value = title, Style = boldStyle });

            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "#", Style = boldCenterStyle });
            sheet.Cells.Add(new XlsxCell(1, 1) { Value = "Name", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(2, 1) { Value = "ID #", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(3, 1) { Value = "Email", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(4, 1) { Value = "Password", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(5, 1) { Value = "Exam Login URL", Style = boldStyle });

            for (var i = 0; i < regsCount; i++)
            {
                var reg = @event.Registrations[i];
                sheet.Cells.Add(new XlsxCell(0, i + 2) { Value = reg.Number, Style = normalCenterStyle });
                sheet.Cells.Add(new XlsxCell(1, i + 2) { Value = reg.UserFullName, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(2, i + 2) { Value = reg.PersonCode, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(3, i + 2) { Value = reg.UserEmail, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(4, i + 2) { Value = reg.Password, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(5, i + 2) { Value = reg.ExamLoginUrl, Style = normalStyle });
            }

            var summary = $"Total registrations: {regsCount}";
            sheet.Cells.Add(new XlsxCell(0, regsCount + 3, 6) { Value = summary, Style = boldStyle });

            return XlsxWorksheet.GetBytes(sheet);
        }

        internal static EventInfo GetEventInfo(Guid eventId)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var examLoginUrl = UrlHelper.GetAbsoluteUrl(
                ServiceLocator.AppSettings.Partition.Domain,
                ServiceLocator.AppSettings.Environment,
                "/ui/lobby/events/login",
                CurrentSessionState.Identity.Organization.Code);

            var @event = ServiceLocator.EventSearch.GetEvent(eventId, x => x.Registrations.Select(y => y.Candidate));

            return new EventInfo
            {
                EventTitle = @event.EventTitle,
                EventClassCode = @event.EventClassCode,
                EventStart = @event.EventScheduledStart.FormatDateOnly(timeZone),
                EventEnd = @event.EventScheduledEnd.HasValue
                    ? @event.EventScheduledEnd.FormatDateOnly(timeZone)
                    : "N/A",
                ExamLoginUrl = examLoginUrl,
                Registrations = @event.Registrations
                    .Where(x => x.ApprovalStatus == "Registered")
                    .OrderBy(x => x.RegistrationSequence)
                    .ThenBy(x => x.RegistrationRequestedOn)
                    .ThenBy(x => x.CandidateIdentifier)
                    .Select(x => new RegistrationItem(x)
                    {
                        ExamLoginUrl = examLoginUrl
                    })
                    .ToList()
            };
        }
    }
}