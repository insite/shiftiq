using System;
using System.Linq;

using InSite.Application.Surveys.Read;
using InSite.Domain.Foundations;
using InSite.Domain.Surveys.Forms;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public class SubmissionSessionState
    {
        public bool Debug { get; set; }
        public string Language => CookieTokenModule.Current.Language;

        public Guid FormIdentifier { get; set; } = Guid.Empty;
        public Guid UserIdentifier { get; set; } = Shift.Constant.UserIdentifiers.Someone;
        public Guid SessionIdentifier { get; set; } = Guid.Empty;

        public int PageNumber { get; set; }
        public int PageCount { get; set; }

        public Guid? Question { get; set; }

        public bool IsValid { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsLocked { get; set; }
        public bool IsAdminAccess { get; set; }
        public bool IsRespondentAnonymous { get; set; }
        public bool IsRespondentValid { get; set; }

        public SurveyForm Survey { get; set; }
        public QResponseSession Session { get; set; }
        public ISurveyResponse[] Sessions { get; set; }
        public ISecurityFramework Identity => CurrentSessionState.Identity;
        public Persistence.User Respondent { get; set; }

        public int GetResponseNumber(Guid response)
        {
            var first = Sessions.FirstOrDefault(x => x.ResponseSessionIdentifier == response);
            return 1 + Array.IndexOf(Sessions, first);
        }

        public void ReloadCurrentSession()
        {
            Session = SessionIdentifier == Guid.Empty
                ? null
                : ServiceLocator.SurveySearch.GetResponseSession(
                    SessionIdentifier, x => x.QResponseAnswers, x => x.QResponseOptions);
        }
    }
}
