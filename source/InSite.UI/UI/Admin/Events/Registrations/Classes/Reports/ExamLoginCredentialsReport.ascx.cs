using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
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
            public string FormTitle { get; set; }
            public string Password { get; set; }
            public string ExamLoginUrl { get; set; }

            public RegistrationItem(QRegistration registration)
            {
                Number = registration.RegistrationSequence ?? 0;
                FormTitle = registration.Form?.FormTitle;
                Password = registration.RegistrationPassword;

                var candidate = registration.Candidate;
                if (candidate == null)
                    return;

                UserFullName = candidate.UserFullName;
                PersonCode = candidate.PersonCode;
                UserEmail = candidate.UserEmail;
            }

            public RegistrationItem(QRegistration registration, QRegistrationExamForm examForm)
                : this(registration)
            {
                FormTitle = examForm.Form?.FormTitle;
                Password = examForm.Password;
            }
        }

        internal class EventInfo
        {
            public string EventTitle { get; set; }
            public string EventStart { get; set; }
            public string EventEnd { get; set; }
            public string EventClassCode { get; set; }
            public string ExamLoginUrl { get; set; }

            public bool ShowAssessmentForm { get; set; }
            public int RegistrationCount { get; set; }

            public List<RegistrationItem> Registrations { get; set; }
        }

        private const string ReportTitle = "Exam Login Credentials";

        protected bool ShowAssessmentForm { get; set; }

        internal static byte[] GetPdf(EventInfo @event)
        {
            using (var page = new Page())
            {
                page.EnableEventValidation = false;
                page.EnableViewState = false;

                var report = (ExamLoginCredentialsReport)page.LoadControl("~/UI/Admin/Events/Registrations/Classes/Reports/ExamLoginCredentialsReport.ascx");

                report.PageTitle.InnerText = ReportTitle;
                report.ShowAssessmentForm = @event.ShowAssessmentForm;
                report.TotalRegistrations.Text = @event.RegistrationCount.ToString();

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
            var rowCount = @event.Registrations.Count;
            var showForm = @event.ShowAssessmentForm;
            var columnCount = showForm ? 7 : 6;

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left, VAlign = XlsxCellVAlign.Center };
            var normalCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center };

            var sheet = new XlsxWorksheet(ReportTitle);

            var formColumn = showForm ? 4 : -1;
            var passwordColumn = showForm ? 5 : 4;
            var loginUrlColumn = showForm ? 6 : 5;

            sheet.Columns[0].Width = 5;
            sheet.Columns[1].Width = 30;
            sheet.Columns[2].Width = 15;
            sheet.Columns[3].Width = 35;

            if (showForm)
                sheet.Columns[formColumn].Width = 30;

            sheet.Columns[passwordColumn].Width = 20;
            sheet.Columns[loginUrlColumn].Width = 45;

            var title = $"Exam Login Credentials \n" +
                $"{@event.EventTitle} #{@event.EventClassCode} ({@event.EventStart} - {@event.EventEnd})";

            sheet.Rows[0].Height = 30;
            sheet.Cells.Add(new XlsxCell(0, 0, columnCount) { Value = title, Style = boldStyle });

            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "#", Style = boldCenterStyle });
            sheet.Cells.Add(new XlsxCell(1, 1) { Value = "Name", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(2, 1) { Value = "ID #", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(3, 1) { Value = "Email", Style = boldStyle });

            if (showForm)
                sheet.Cells.Add(new XlsxCell(formColumn, 1) { Value = "Assessment Form", Style = boldStyle });

            sheet.Cells.Add(new XlsxCell(passwordColumn, 1) { Value = "Password", Style = boldStyle });
            sheet.Cells.Add(new XlsxCell(loginUrlColumn, 1) { Value = "Exam Login URL", Style = boldStyle });

            for (var i = 0; i < rowCount; i++)
            {
                var reg = @event.Registrations[i];
                sheet.Cells.Add(new XlsxCell(0, i + 2) { Value = reg.Number, Style = normalCenterStyle });
                sheet.Cells.Add(new XlsxCell(1, i + 2) { Value = reg.UserFullName, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(2, i + 2) { Value = reg.PersonCode, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(3, i + 2) { Value = reg.UserEmail, Style = normalStyle });

                if (showForm)
                    sheet.Cells.Add(new XlsxCell(formColumn, i + 2) { Value = reg.FormTitle, Style = normalStyle });

                sheet.Cells.Add(new XlsxCell(passwordColumn, i + 2) { Value = reg.Password, Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(loginUrlColumn, i + 2) { Value = reg.ExamLoginUrl, Style = normalStyle });
            }

            var summary = $"Total registrations: {@event.RegistrationCount}";
            sheet.Cells.Add(new XlsxCell(0, rowCount + 3, columnCount) { Value = summary, Style = boldStyle });

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

            var showAssessmentForm = CurrentSessionState.Identity.Organization.Toolkits?.Events?.AllowMultipleAssessmentForms == true;

            var includes = new List<Expression<Func<QEvent, object>>>
            {
                x => x.Registrations.Select(y => y.Candidate)
            };

            if (showAssessmentForm)
            {
                includes.Add(x => x.Registrations.Select(y => y.Form));
                includes.Add(x => x.Registrations.Select(y => y.ExamForms.Select(z => z.Form)));
            }

            var @event = ServiceLocator.EventSearch.GetEvent(eventId, includes.ToArray());

            var registrations = @event.Registrations
                .Where(x => x.ApprovalStatus == "Registered")
                .OrderBy(x => x.RegistrationSequence)
                .ThenBy(x => x.RegistrationRequestedOn)
                .ThenBy(x => x.CandidateIdentifier)
                .ToList();

            return new EventInfo
            {
                EventTitle = @event.EventTitle,
                EventClassCode = @event.EventClassCode,
                EventStart = @event.EventScheduledStart.FormatDateOnly(timeZone),
                EventEnd = @event.EventScheduledEnd.HasValue
                    ? @event.EventScheduledEnd.FormatDateOnly(timeZone)
                    : "N/A",
                ExamLoginUrl = examLoginUrl,
                ShowAssessmentForm = showAssessmentForm,
                RegistrationCount = registrations.Count,
                Registrations = registrations
                    .SelectMany(x => GetRegistrationItems(x, showAssessmentForm, examLoginUrl))
                    .ToList()
            };
        }

        private static IEnumerable<RegistrationItem> GetRegistrationItems(QRegistration registration, bool showAssessmentForm, string examLoginUrl)
        {
            yield return new RegistrationItem(registration) { ExamLoginUrl = examLoginUrl };

            if (!showAssessmentForm || registration.ExamFormIdentifier == null)
                yield break;

            foreach (var examForm in registration.ExamForms.OrderBy(x => x.Sequence))
                yield return new RegistrationItem(registration, examForm) { ExamLoginUrl = examLoginUrl };
        }
    }
}
