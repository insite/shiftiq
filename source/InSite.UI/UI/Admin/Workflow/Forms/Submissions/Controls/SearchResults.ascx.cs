using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Workflow.Forms.Submissions.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QResponseSessionFilter>
    {
        private class DataItem
        {
            public Guid ResponseSessionIdentifier { get; }
            public Guid SurveyFormIdentifier { get; }
            public DateTimeOffset? ResponseSessionStarted { get; }
            public DateTimeOffset? ResponseSessionCompleted { get; }
            public bool ResponseIsLocked { get; }

            public string ResponseIsLockedHtml
            {
                get
                {
                    if (ResponseIsLocked)
                        return $"<div class='text-danger fs-xs'><i class='far fa-lock me-1'></i>Locked</div>";

                    return $"<div class='text-success fs-xs'><i class='far fa-lock-open me-1'></i>Unlocked</div>";
                }
            }

            public string SurveyName { get; }
            public int? SurveyNumber { get; }
            public bool SurveyIsConfidential { get; }

            public Guid? RespondentUserIdentifier { get; }
            public string RespondentName { get; }
            public string RespondentEmail { get; }
            public string RespondentHtml { get; set; }

            public string GroupName { get; set; }
            public string PeriodName { get; set; }

            public string FirstComment { get; set; }
            public string FirstSelection { get; set; }

            public string FirstCommentHtml
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(FirstComment))
                        return string.Empty;

                    var snip = StringHelper.Snip(FirstComment, 100);
                    return $"<div class='text-info fs-xs' title='First comment'><i class='far fa-comment me-1'></i>{snip}</div>";
                }
            }

            public string FirstSelectionHtml
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(FirstSelection))
                        return string.Empty;

                    var snip = StringHelper.Snip(FirstSelection, 100);
                    return $"<div class='text-success fs-xs' title='First selection'><i class='far fa-circle-check me-1'></i>{snip}</div>";
                }
            }

            public DataItem(ISurveyResponse entity)
            {
                ResponseSessionIdentifier = entity.ResponseSessionIdentifier;
                SurveyFormIdentifier = entity.SurveyFormIdentifier;
                ResponseSessionStarted = entity.ResponseSessionStarted;
                ResponseSessionCompleted = entity.ResponseSessionCompleted;
                ResponseIsLocked = entity.ResponseIsLocked;

                {
                    SurveyName = entity.SurveyName;
                    SurveyNumber = entity.SurveyNumber;
                    SurveyIsConfidential = entity.SurveyIsConfidential;
                }

                {
                    RespondentUserIdentifier = entity.RespondentUserIdentifier;
                    RespondentName = entity.RespondentName;
                    RespondentEmail = entity.RespondentEmail;
                }

                GroupName = entity.GroupName;
                PeriodName = entity.PeriodName;

                FirstComment = entity.FirstCommentText;
                FirstSelection = entity.FirstSelectionText;
            }
        }

        private bool? _canChangeResponse;
        protected bool CanChangeResponse
        {
            get
            {
                if (_canChangeResponse == null)
                    _canChangeResponse = Identity.IsGranted(PermissionNames.Admin_Forms_Submissions_Change);

                return _canChangeResponse.Value;
            }
        }

        public string ScreenParams { get; set; }

        protected override int SelectCount(QResponseSessionFilter filter)
        {
            return ServiceLocator.SurveySearch.CountResponseSessions(filter);
        }

        protected override IListSource SelectData(QResponseSessionFilter filter)
        {
            var sessions = ServiceLocator.SurveySearch
                .GetResponseSessions(filter)
                .Select(x => new DataItem(x)).ToList();

            foreach (var session in sessions)
            {
                var html = string.Empty;
                var isConfidential = session.SurveyIsConfidential;
                if (session.RespondentUserIdentifier == UserIdentifiers.Someone)
                {
                    html = UserNames.Someone;
                }
                else if (Identity.IsOperator || !isConfidential)
                {
                    html = session.RespondentName;
                    if (session.RespondentUserIdentifier.HasValue)
                        html = $"<a href='/ui/admin/contacts/people/edit?contact={session.RespondentUserIdentifier}'>{session.RespondentName}</a>"
                             + $"<div class='form-text'><a href='mailto:{session.RespondentEmail}'>{session.RespondentEmail}</a></div>";
                }
                else
                {
                    html = "**********";
                }
                session.RespondentHtml = html;
            }

            MainPanel.Visible = true;

            return sessions.ToSearchResult();
        }

        public void LoadForEmployer(QResponseSessionFilter filter)
        {
            Filter = filter;
            MainPanel.Visible = RowCount != 0;
        }

        protected string GetLocalDate(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }

        protected string GetScreenUrl(string url)
        {
            var item = Page.GetDataItem();
            var sessionId = DataBinder.Eval(item, "ResponseSessionIdentifier");

            return new ReturnUrl(ScreenParams).GetRedirectUrl($"{url}?session={sessionId}");
        }
    }
}