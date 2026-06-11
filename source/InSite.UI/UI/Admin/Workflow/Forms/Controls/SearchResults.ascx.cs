using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QSurveyFormFilter>
    {
        public class ExportItem
        {
            public Guid SurveyFormIdentifier { get; set; }
            public Guid? SurveyMessageInvitation { get; set; }
            public Guid? SurveyMessageResponseCompleted { get; set; }
            public Guid? SurveyMessageResponseConfirmed { get; set; }
            public Guid? SurveyMessageResponseStarted { get; set; }

            public string SurveyFormHook { get; set; }
            public string SurveyFormLanguage { get; set; }
            public string SurveyFormLanguageTranslations { get; set; }
            public string SurveyFormName { get; set; }
            public string SurveyFormTitle { get; set; }
            public string SurveyFormStatus { get; set; }

            public bool EnableUserConfidentiality { get; set; }
            public string UserFeedback { get; set; }
            public bool RequireUserAuthentication { get; set; }
            public bool RequireUserIdentification { get; set; }
            public bool DisplaySummaryChart { get; set; }

            public int AssetNumber { get; set; }
            public int? SurveyFormDurationMinutes { get; set; }
            public int? ResponseLimitPerUser { get; set; }

            public DateTimeOffset? SurveyFormClosed { get; set; }
            public DateTimeOffset? SurveyFormLocked { get; set; }
            public DateTimeOffset? SurveyFormOpened { get; set; }

            public DateTimeOffset Created { get; set; }
            public string CreatedBy { get; set; }

            public DateTimeOffset LastChangeTime { get; set; }
            public string LastChangeType { get; set; }
            public string LastChangeUser { get; set; }

            public int PageCount { get; set; }
            public int QuestionCount { get; set; }
            public int BranchCount { get; set; }
            public int ConditionCount { get; set; }

            public bool HasWorkflowConfiguration { get; set; }
        }

        protected override int SelectCount(QSurveyFormFilter filter)
        {
            return ServiceLocator.SurveySearch.CountSurveyForms(filter);
        }

        protected override IListSource SelectData(QSurveyFormFilter filter)
        {
            if (filter.Paging == null)
                return new List<QSurveyForm>().ToSearchResult();

            return ServiceLocator.SurveySearch
                .GetSurveyForms(filter, x => x.LastChangeUserEntity, x => x.CreatedByUser)
                .ToSearchResult();
        }

        public override IListSource GetExportData(QSurveyFormFilter filter, bool empty)
        {
            if (empty)
                return (new ExportItem[0]).ToSearchResult();

            var data = ServiceLocator.SurveySearch
                .GetSurveyForms(filter, x => x.LastChangeUserEntity, x => x.CreatedByUser);

            var result = data.Select(x => new ExportItem
                {
                    SurveyFormIdentifier = x.SurveyFormIdentifier,
                    SurveyMessageInvitation = x.SurveyMessageInvitation,
                    SurveyMessageResponseCompleted = x.SurveyMessageResponseCompleted,
                    SurveyMessageResponseConfirmed = x.SurveyMessageResponseConfirmed,
                    SurveyMessageResponseStarted = x.SurveyMessageResponseStarted,
                    SurveyFormHook = x.SurveyFormHook,
                    SurveyFormLanguage = x.SurveyFormLanguage,
                    SurveyFormLanguageTranslations = x.SurveyFormLanguageTranslations,
                    SurveyFormName = x.SurveyFormName,
                    SurveyFormTitle = x.SurveyFormTitle,
                    SurveyFormStatus = x.SurveyFormStatus,
                    EnableUserConfidentiality = x.EnableUserConfidentiality,
                    UserFeedback = x.UserFeedback,
                    RequireUserAuthentication = x.RequireUserAuthentication,
                    RequireUserIdentification = x.RequireUserIdentification,
                    DisplaySummaryChart = x.DisplaySummaryChart,
                    AssetNumber = x.AssetNumber,
                    SurveyFormDurationMinutes = x.SurveyFormDurationMinutes,
                    ResponseLimitPerUser = x.ResponseLimitPerUser,
                    SurveyFormClosed = x.SurveyFormClosed,
                    SurveyFormLocked = x.SurveyFormLocked,
                    SurveyFormOpened = x.SurveyFormOpened,
                    Created = x.Created,
                    CreatedBy = x.CreatedByUser?.FullName,
                    LastChangeTime = x.LastChangeTime,
                    LastChangeType = x.LastChangeType,
                    LastChangeUser = x.LastChangeUserEntity?.FullName,
                    PageCount = x.PageCount,
                    QuestionCount = x.QuestionCount,
                    BranchCount = x.BranchCount,
                    ConditionCount = x.ConditionCount,
                    HasWorkflowConfiguration = x.HasWorkflowConfiguration
                })
                .ToList();

            return result.ToSearchResult();
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

        protected string GetDataTimeHtml(DateTimeOffset? date, string userFullName)
        {
            var builder = new StringBuilder();

            if (date.HasValue)
                builder.Append(TimeZones.Format(date.Value, User.TimeZone, true));

            if(string.IsNullOrEmpty(userFullName))
                return builder.ToString();

            if (builder.Length > 0) 
                builder.Append("<br/>");

            builder.Append($"<small class=\"text-body-secondary\">by {userFullName}</small>");

            return builder.ToString();
        }
    }
}
