using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Surveys.Read;
using InSite.Persistence;
using InSite.UI.Portal.Workflow.Forms.Models;
using InSite.Web.Routing;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public class SubmissionSessionControl : UserControl
    {
        protected SubmitPage AppPage => Page as SubmitPage;

        public SubmissionSessionState Current { get; set; }

        private IEnumerable<string> Errors { get; set; }

        public SubmissionSessionNavigator Navigator { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Errors = LoadCurrentState();

            if (!Page.IsPostBack)
                BindControls(Errors);
        }

        private IEnumerable<string> LoadCurrentState()
        {
            Current = new SubmissionSessionState();
            Navigator = new SubmissionSessionNavigator();

            var qs = new SubmissionSessionQueryString(Page.RouteData, Page.Request.QueryString);

            var errors = qs.Session != Guid.Empty
                ? LoadPropertiesFromSession(qs)
                : LoadPropertiesFromUrl(qs);

            if (!errors.Any())
                errors = LoadCurrentStateObjects(Current, Navigator);

            Current.IsValid = !errors.Any();

            return errors;
        }

        private IEnumerable<string> LoadPropertiesFromSession(SubmissionSessionQueryString queryString)
        {
            Current.Debug = queryString.Debug;
            Current.PageNumber = queryString.PageNumber;
            Current.Question = queryString.Question;

            var query = ServiceLocator.SurveySearch.GetResponseSession(queryString.Session);
            if (query == null || query.OrganizationIdentifier != Current.Identity.Organization.OrganizationIdentifier)
                return new[] { "<strong>Invalid URL:</strong> A valid form submission session identifier is required." };

            Current.IsCompleted = query.ResponseSessionCompleted.HasValue;
            Current.IsLocked = query.ResponseIsLocked;
            Current.IsAdminAccess = Current.Identity.IsGranted(PermissionNames.Admin_Forms_Submissions_Change);
            Current.IsRespondentAnonymous = query.RespondentUserIdentifier == Guid.Empty
                || query.RespondentUserIdentifier == Shift.Constant.UserIdentifiers.Someone;
            Current.IsRespondentValid = Current.Identity.User != null
                && query.RespondentUserIdentifier == Current.Identity.User.Identifier;

            Current.SessionIdentifier = query.ResponseSessionIdentifier;
            Current.FormIdentifier = query.SurveyFormIdentifier;
            Current.UserIdentifier = query.RespondentUserIdentifier;

            return new string[0];
        }

        private IEnumerable<string> LoadPropertiesFromUrl(SubmissionSessionQueryString queryString)
        {
            Current.Debug = queryString.Debug;
            Current.PageNumber = queryString.PageNumber;
            Current.Question = queryString.Question;

            if (queryString.Verb != "search" && queryString.FormAsset == null && queryString.FormIdentifier == null)
                return new[] { "<strong>Invalid URL:</strong> A valid form identifier (or asset number) is required." };

            if (queryString.FormAsset != null)
            {
                var query = ServiceLocator.SurveySearch.GetSurveyFormByAsset(Current.Identity.Organization.OrganizationIdentifier, queryString.FormAsset.Value);
                if (query == null)
                    return new[] { $"<strong>Form Not Found:</strong> There is no form with this asset number ({queryString.FormAsset.Value})." };

                Current.FormIdentifier = query.SurveyFormIdentifier;
            }
            else if (queryString.FormIdentifier != null)
            {
                Current.FormIdentifier = queryString.FormIdentifier.Value;
            }

            if (queryString.User != null)
            {
                Current.UserIdentifier = queryString.User.Value;
            }
            else if (Current.Identity.IsAuthenticated)
            {
                Current.UserIdentifier = Current.Identity.User.UserIdentifier;
            }

            return new string[0];
        }

        public static IEnumerable<string> LoadCurrentStateObjects(SubmissionSessionState current, SubmissionSessionNavigator navigator)
        {
            if (current.UserIdentifier != Shift.Constant.UserIdentifiers.Someone)
            {
                current.Sessions = ServiceLocator.SurveySearch
                    .GetResponseSessions(current.FormIdentifier, current.UserIdentifier)
                    .OrderByDescending(x => x.ResponseSessionCreated)
                    .ToArray();
            }
            else
            {
                current.Sessions = new VResponse[0];
            }

            current.ReloadCurrentSession();

            current.Survey = ServiceLocator.SurveySearch.GetSurveyState(current.FormIdentifier)?.Form;

            if (current.SessionIdentifier != Guid.Empty)
                if (current.Survey == null || current.Survey.Tenant != current.Identity.Organization.OrganizationIdentifier)
                    return new[] { "<strong>Invalid URL:</strong> There is no form matching the submission session." };

            if (current.Survey != null)
            {
                current.PageCount = current.Survey.CountPages();
                navigator.Initialize(current.Survey);
            }

            current.Respondent = UserSearch.Select(current.UserIdentifier);

            return new string[0];
        }

        protected virtual void BindControls(IEnumerable<string> errors)
        {
            if (errors.Any())
            {
                if (FindControl("ErrorAlert") is InSite.Common.Web.UI.Alert alert)
                {
                    foreach (var error in errors)
                        alert.AddMessage(AlertType.Error, error);
                }

                return;
            }

            if (Request.IsAuthenticated)
            {
                var homeUrl = GetCourseUrl() ?? RelativeUrl.PortalHomeUrl;

                AppPage.AddBreadcrumb("Home", homeUrl);

                if (Current.Survey != null && Current.Survey.RequireUserIdentification && (Current.Respondent != null && Current.Respondent.UserIdentifier != Shift.Constant.UserIdentifiers.Someone))
                {
                    AppPage.AddBreadcrumb("Forms", "/ui/portal/workflow/forms/submit/search");
                    if (Current.Survey != null)
                    {
                        AppPage.AddBreadcrumb($"{Current.Survey.Content?.Title?.Text[Current.Language]}", $"/ui/portal/workflow/forms/submit/launch?form={Current.Survey.Asset}&user={Current.UserIdentifier}", false);
                    }
                }
            }

            if (AppPage.Route.Name.Contains("/answer/"))
            {
                var n = Current.GetResponseNumber(Current.Session.ResponseSessionIdentifier);
                var ordinal = n > 1 ? n.ToOrdinalWords().Titleize() + " " : string.Empty;
                AppPage.AddBreadcrumb($"{ordinal}Submission", null);
                AppPage.AddBreadcrumb($"Page {Current.PageNumber} of {Current.PageCount}", null);
            }
            else if (AppPage.Route.Name.Contains("/confirm/"))
            {
                AppPage.AddBreadcrumb($"Confirm", null);
            }
            else if (AppPage.Route.Name.Contains("/complete/"))
            {
                AppPage.AddBreadcrumb($"Complete", null);
            }
            else if (AppPage.Route.Name.Contains("/review/"))
            {
                AppPage.AddBreadcrumb($"Review", null);
            }

            var title = FindControl("SurveyFormTitle") as HtmlGenericControl;
            title.InnerText = Current.Survey?.Content?.Title?.Text[Current.Language];

            if (!IsPostBack && Current.Debug)
            {
                var info = LoadControl("~/UI/Portal/Workflow/Forms/Controls/FormDebugInfo.ascx");
                ((FormDebugInfo)info).Bind(Current.Survey, Current.Respondent, Current.Session, Current.Sessions, Current.PageNumber);

                var panel = FindControl("DebugPanel") as Panel;
                if (panel != null)
                {
                    panel.Visible = true;
                    panel.Controls.Add(info);
                }
            }
        }

        protected string GetCourseUrl()
        {
            var callerData = HttpContext.Current.Request.QueryString["caller"];
            var callerObj = callerData.IsNotEmpty() ? FormCaller.Deserialize(callerData) : null;
            var callerUrl = callerObj?.GetReturnUrl();

            if (callerUrl.IsNotEmpty() || Current.Survey == null)
                return callerUrl;

            var activity = CourseSearch.BindActivityFirst(
                x => new { x.ActivityIdentifier, CourseIdentifier = x.Module.Unit.CourseIdentifier },
                x => x.SurveyFormIdentifier == Current.Survey.Identifier);

            if (activity != null && TGroupPermissionSearch.IsAccessAllowed(activity.ActivityIdentifier, CurrentSessionState.Identity))
                callerUrl = RoutingConfiguration.PortalCourseUrl(activity.CourseIdentifier, activity.ActivityIdentifier);

            return callerUrl;
        }

        public string GetDisplayHtml(string attribute, string @default = null)
            => Markdown.ToHtml(GetDisplayText(attribute, @default));

        public string GetDisplayText(string attribute, string @default = null)
        {
            var token = CookieTokenModule.Current;
            var organization = OrganizationSearch.Select(token.OrganizationCode)?.OrganizationIdentifier ?? Guid.Empty;
            return LabelSearch.GetTranslation(attribute, CurrentSessionState.Identity.Language, organization, false, true, @default);
        }

        protected string LocalizeTime(object time) =>
            ((DateTimeOffset?)time).Format(Current.Respondent.TimeZone, "-");

        protected string Translate(string text) => AppPage.Translate(text);
    }
}
