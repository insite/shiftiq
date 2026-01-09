using System;
using System.Text;
using System.Web.UI;

using InSite.Application.Surveys.Read;
using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public partial class FormDebugInfo : UserControl
    {
        public SurveyForm Survey { get; set; }
        public Persistence.User Respondent { get; set; }
        public QResponseSession ResponseSession { get; set; }
        public ISurveyResponse[] ResponseSessions { get; set; }
        public int PageNumber { get; set; }

        public void Bind(SurveyForm form, Persistence.User respondent, QResponseSession session, ISurveyResponse[] sessions, int page)
        {
            Survey = form;
            Respondent = respondent;
            ResponseSession = session;
            ResponseSessions = sessions ?? new VResponse[0];
            PageNumber = page;
            LastQuestion.Text = "-";

            if (Survey != null && ResponseSession != null)
            {
                if (ResponseSession.LastAnsweredQuestionIdentifier.HasValue)
                {
                    var last = Survey.FindQuestion(ResponseSession.LastAnsweredQuestionIdentifier.Value);
                    LastQuestion.Text = $"Question {last.Sequence} on Page {Survey.GetPageNumber(last.Identifier)}";
                }
            }
        }

        protected string GetRespondentLabels()
        {
            var html = new StringBuilder();

            if (Respondent.UserIdentifier != Guid.Empty && Respondent.UserIdentifier != Shift.Constant.UserIdentifiers.Someone)
                html.Append("<span class='badge bg-success ms-2'>Identified</span>");

            if (CurrentSessionState.Identity.IsAuthenticated)
                html.Append("<span class='badge bg-primary ms-2'>Authenticated</span>");

            return html.ToString();
        }

        protected string LocalizeTime(object time)
        {
            if (time == null)
            {
                return "-";
            }

            return TimeZones.Format((DateTimeOffset)time, Respondent.TimeZone);
        }
    }
}