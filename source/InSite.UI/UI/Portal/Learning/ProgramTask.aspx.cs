using System;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Records.Programs.Tasks
{
    public partial class Launch : PortalBasePage
    {
        private ProgramState _state;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadState();

            if (IsPostBack)
                return;
        }

        private void LoadState()
        {
            if (_state != null)
                return;

            _state = new ProgramState(RouteData, Request.QueryString);
            Guid? taskId = _state.GetTaskId();

            if (!taskId.HasValue)
                HttpResponseHelper.Redirect(GetHomeUrl(), true);

            var task = TaskSearch.SelectFirst(x => x.TaskIdentifier == taskId.Value);

            if (task == null)
                HttpResponseHelper.SendHttp404();

            RedirectCaller(task);
        }

        private void RedirectCaller(TTask task)
        {
            var url = BuildTaskUrl(task);
            var identity = CurrentSessionState.Identity;


            MarkAsViewed(task.ObjectIdentifier, identity.User.UserIdentifier, identity.Organization.Identifier);

            if (url.Length == 0)
                url = GetHomeUrl();

            HttpResponseHelper.Redirect(url);
        }

        private string BuildTaskUrl(TTask task)
        {
            switch(task.ObjectType)
            {
                case "Logbook":
                    return $"/ui/portal/records/logbooks/outline?journalsetup={task.ObjectIdentifier}";
                case "Course":
                    return $"/ui/portal/learning/course/{task.ObjectIdentifier}";
                case "AssessmentForm":
                    return $"/ui/portal/assessments/attempts/start?form={task.ObjectIdentifier}";
                case "Survey":
                    if (CurrentSessionState.Identity.User != null)
                    {
                        var form = ServiceLocator.SurveySearch.GetSurveyState(task.ObjectIdentifier)?.Form;
                        if (form != null)
                            return $"/ui/portal/workflow/forms/submit/launch?form={form.Asset}&user={CurrentSessionState.Identity.User.UserIdentifier}";
                    }
                    break;
                case "Achievement":
                    return $"/ui/portal/record/credentials/learners/search"; ;
            }
            return string.Empty;
        }

        private void MarkAsViewed(Guid objectIdentifier, Guid userIdentifier, Guid organizationIdentifier)
            => ServiceLocator.ProgramStore.TaskViewed(userIdentifier, organizationIdentifier, objectIdentifier);

        private string GetHomeUrl()
        {
            var url = RelativeUrl.PortalHomeUrl;
            return url;
        }
    }
}