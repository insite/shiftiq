using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Humanizer;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QSurveyFormFilter>
    {
        protected override int SelectCount(QSurveyFormFilter filter)
        {
            return ServiceLocator.SurveySearch.CountSurveyForms(filter);
        }

        protected override IListSource SelectData(QSurveyFormFilter filter)
        {
            if (filter.Paging == null)
                return new List<QSurveyForm>().ToSearchResult();

            return ServiceLocator.SurveySearch.GetSurveyForms(filter).ToSearchResult();
        }

        public override IListSource GetExportData(QSurveyFormFilter filter, bool empty)
        {
            return SelectData(filter);
        }

        protected string GetInvitationLink(QSurveyForm surveyForm)
        {
            if (surveyForm == null) return string.Empty;

            var hasAnyMessage =
                surveyForm.SurveyMessageInvitation.HasValue ||
                surveyForm.SurveyMessageResponseCompleted.HasValue ||
                surveyForm.SurveyMessageResponseConfirmed.HasValue ||
                surveyForm.SurveyMessageResponseStarted.HasValue;

            if (!hasAnyMessage || surveyForm.SurveyFormIdentifier == Guid.Empty)
                return string.Empty;

            return $"<a href='/ui/admin/workflow/forms/outline?survey={surveyForm.SurveyFormIdentifier}&panel=messages&tab=Invitation'><i class=\"icon far fa-envelope-open-text\"></i></a>";
        }

        protected string GetSummaryHtml(Guid id)
        {
            var summary = ServiceLocator.SurveySearch.GetSurveyResponseSummary(id);
            if (summary != null)
                return GetHtml(summary);

            return null;
        }

        private string GetHtml(VSurveyResponseSummary summary)
        {
            var builder = new StringBuilder();

            builder.AppendLine("<table class=\"table-summary\">");
            builder.AppendLine("<tbody>");

            builder.AppendLine("<tr>");
            builder.AppendLine($"<td class=\"text-end\">{summary.SumResponseStartCount}</td>");
            builder.AppendLine($"<td>Starts beginning {GetDateString(summary.MinResponseStarted)}</td>");
            builder.AppendLine("</tr>");

            builder.AppendLine("<tr>");
            builder.AppendLine($"<td class=\"text-end\">{summary.SumResponseCompleteCount}</td>");
            builder.AppendLine($"<td>Complete ending {GetDateString(summary.MaxResponseCompleted)}</td>");
            builder.AppendLine("</tr>");

            if (summary.AvgResponseTimeTaken.HasValue)
            {
                var minutes = "minute".ToQuantity((int)(summary.AvgResponseTimeTaken), "N0");
                builder.AppendLine("<tr>");
                builder.AppendLine($"<td class=\"text-end\"></td>");
                builder.AppendLine($"<td>Average time taken = {minutes}</td>");
                builder.AppendLine("</tr>");
            }

            builder.AppendLine("</tbody>");
            builder.AppendLine("</table>");

            return builder.ToString();
        }

        private string GetDateString(DateTimeOffset? date)
        {
            if (date.HasValue)
                return TimeZones.Format(date.Value, Identity.User.TimeZone, true, false);

            return null;
        }

        protected string GetDataTimeHtml(DateTimeOffset? date, Guid userId)
        {
            var builder = new StringBuilder();

            if (date.HasValue)
                builder.Append(TimeZones.Format(date.Value, User.TimeZone, true));

            var user = ServiceLocator.UserSearch.GetUser(userId);

            if(user == null)
                return builder.ToString();

            if (builder.Length > 0) 
                builder.Append("<br/>");

            builder.Append($"<small class=\"text-body-secondary\">by {user.FullName}</small>");

            return builder.ToString();
        }
    }
}