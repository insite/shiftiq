using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class ReportSubmissionSearchResults : SearchResultsGridViewController<QResponseSessionFilter>
    {
        protected override int SelectCount(QResponseSessionFilter filter)
        {
            return ServiceLocator.SurveySearch.CountResponseSessions(filter);
        }

        protected override IListSource SelectData(QResponseSessionFilter filter)
        {
            if (filter.Paging == null)
                return new List<QSurveyForm>().ToSearchResult();

            return ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .Select(x => new
                {
                    ResponseSessionIdentifier = x.ResponseSessionIdentifier,
                    SurveyFormIdentifier = x.SurveyFormIdentifier,
                    SurveyName = x.SurveyName,
                    SurveyNumber = x.SurveyNumber,
                    RespondentUserIdentifier = x.RespondentUserIdentifier,
                    RespondentName = x.RespondentName,
                    SurveyIsConfidential = x.SurveyIsConfidential,
                    RespondentEmail = x.RespondentEmail,
                    ResponseIsLocked = x.ResponseIsLocked,
                    ResponseSessionStarted = x.ResponseSessionStarted,
                    ResponseSessionCompleted = x.ResponseSessionCompleted,
                })
                .ToList()
                .ToSearchResult();
        }

        protected string GetTitle(Guid surveyIdentifier)
        {
            return ServiceLocator.ContentSearch.GetTitleText(surveyIdentifier);
        }

        protected string GetInvitationLink(Guid? surveyMessageInvitation)
        {
            if (surveyMessageInvitation.HasValue)
                return $"<a href='/ui/admin/messages/outline?message={surveyMessageInvitation}'><i class='icon far fa-envelope-open-text'></i></a>";

            return null;
        }
    }
}