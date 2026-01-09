using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Reports
{
    public abstract class BaseReportControl : BaseUserControl
    {
        protected class CriteriaItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public abstract string ReportTitle { get; }
        public abstract string ReportFileName { get; }

        public abstract byte[] GetXlsx(QEventFilter filter);
        public abstract byte[] GetPdf(QEventFilter filter);

        protected static List<CriteriaItem> GetCriteriaItems(QEventFilter filter)
        {
            var timeZone = CurrentSessionState.Identity.User.TimeZone;
            var result = new List<CriteriaItem>();

            if (filter.EventScheduledSince.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Since", Value = filter.EventScheduledSince.Value.Format(timeZone) });

            if (filter.EventScheduledBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Scheduled Before", Value = filter.EventScheduledBefore.Value.Format(timeZone) });

            if (filter.DistributionExpectedSince.HasValue)
                result.Add(new CriteriaItem { Name = "Distribution Expected Since", Value = filter.DistributionExpectedSince.Value.Format(timeZone) });

            if (filter.DistributionExpectedBefore.HasValue)
                result.Add(new CriteriaItem { Name = "Distribution Expected Before", Value = filter.DistributionExpectedBefore.Value.Format(timeZone) });

            if (filter.AchievementIdentifier.HasValue)
            {
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(filter.AchievementIdentifier.Value);
                if (achievement != null)
                    result.Add(new CriteriaItem { Name = "Achievement", Value = achievement.AchievementTitle });
            }

            if (filter.EventTitle.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Class Title", Value = filter.EventTitle });

            if (filter.Venue.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Class Venue", Value = filter.Venue });

            if (filter.EventPublicationStatus.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Publication status", Value = filter.EventPublicationStatus });

            if (filter.IsOpen.HasValue)
                result.Add(new CriteriaItem { Name = "Class Registration Status", Value = filter.IsOpen.Value ? "Open" : "Closed" });

            if (filter.CommentKeyword.IsNotEmpty())
                result.Add(new CriteriaItem { Name = "Comment", Value = filter.CommentKeyword });

            return result;
        }

        protected byte[] BuildPdf(PageOrientationType pageOrientation, int width, int height, string reportName)
        {
            var date = DateTimeOffset.Now.FormatDateOnly(User.TimeZone);

            var siteContent = new StringBuilder();
            using (var stringWriter = new StringWriter(siteContent))
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    RenderControl(htmlWriter);
            }

            var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath)
            {
                PageOrientation = pageOrientation,
                Viewport = new HtmlConverterSettings.ViewportSize(width, height),
                MarginTop = 22,
                MarginBottom = 22,
                Dpi = 240,

                HeaderTextLeft = reportName,
                HeaderFontName = "Calibri",
                HeaderFontSize = 19,
                HeaderSpacing = 7.8f,

                FooterTextLeft = reportName,
                FooterTextCenter = date,
                FooterTextRight = "Page [page] of [topage]",
                FooterFontName = "Calibri",
                FooterFontSize = 10,
                FooterSpacing = 8.1f,
            };

            return HtmlConverter.HtmlToPdf(siteContent.ToString(), settings);
        }
    }
}