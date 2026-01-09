using System;
using System.Web;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Lobby;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Sdk.UI;

using AccessControl = InSite.Domain.Foundations.AccessControl;
using UserModel = InSite.Domain.Foundations.User;

namespace InSite.UI.Layout.Portal
{
    public class PortalBasePage : Lobby.LobbyBasePage
    {
        public Common.Controls.Navigation.Navigator Navigator { get; private set; }

        protected PortalMaster PortalMaster => Master as PortalMaster;

        private const string itaExamEventAuthentication = "/ui/lobby/events/login";

        static new protected UserModel User => Identity.User;
        public bool IsAuthenticationRequired { get; set; } = true;

        private Guid? FolderId => Guid.TryParse(Request.QueryString["folder"], out var pageId) ? pageId : (Guid?)null;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            RequireAuthentication();

            Navigator = new Common.Controls.Navigation.Navigator(Request);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void AutoBindFolderHeader(BreadcrumbItem create = null, string qualifier = null, string linkTitle = null, string[] parentLinkTitles = null)
        {
            PageHelper.AutoBindFolderHeader(this, create, FolderId, qualifier, linkTitle, parentLinkTitles);
        }

        public string AddFolderToUrl(string url)
        {
            if (FolderId == null || string.IsNullOrEmpty(url))
                return url;

            var separator = url.IndexOf('?') >= 0 ? "&" : "?";

            return $"{url}{separator}folder={FolderId}";
        }

        private void CheckMfa()
        {
            const string url = "/ui/portal/identity/authenticate";

            // If the user is not authenticated then skip the MFA check.
            if (!Identity.IsAuthenticated || Identity.User == null)
                return;

            // Avoid an infinite redirect loop.
            if (Request.Url.AbsolutePath == url)
                return;

            // Redirect to the MFA page only if MFA authentication is enabled and OTP mode is not activated.
            if (User.MultiFactorAuthentication && User.ActiveOtpMode == OtpModes.None)
                Response.Redirect($"{url}?force=true");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Identity == null)
                HttpResponseHelper.Redirect("/");

            if (!IsMultifactorAuthenticationDisabled())
                CheckMfa();

            if (IsPostBack)
                return;

            ApplyAccessControl();
            ApplyNavContentControl();
        }

        private void ApplyNavContentControl()
        {
            if (Page.Master is PortalMaster b)
            {
                if (!b.IsSalesReady)
                {
                    b.RenderHelpContent(ActionModel);
                    ShowSideContent(b);
                    return;
                }

                ApplySalesReadyNavContentControl(b);
            }
        }

        private void ApplySalesReadyNavContentControl(PortalMaster b)
        {
            if (!Identity.IsAuthenticated)
            {
                b.HideSideContent();
                b.SidebarVisible(false);
                b.HideBreadcrumbsAndTitle();
            }
            else
            {
                b.RenderHelpContent(ActionModel);
                ShowSideContent(b);
            }
        }

        private void ShowSideContent(PortalMaster master)
        {
            // Find the ContentPlaceholder control in the master page
            ContentPlaceHolder placeholder = master.FindControl("SideContent") as ContentPlaceHolder;

            // Check if the Content control exists within the ContentPlaceholder
            bool hasContent = (placeholder != null && placeholder.HasControls());

            if (!hasContent)
            {
                master.HideSideContent();
                master.SidebarVisible(false);
            }
        }

        #region Access Control

        public virtual AccessControl Access
        {
            get
            {
                if (ViewState[nameof(AccessControl)] == null)
                    ViewState[nameof(AccessControl)] = new AccessControl();
                return (AccessControl)ViewState[nameof(AccessControl)];
            }
            set => ViewState[nameof(AccessControl)] = value;
        }

        private static bool AllowUnauthenticatedUsers(string url)
            => StringHelper.Equals(url, itaExamEventAuthentication);

        public virtual void ApplyAccessControl()
        {
            if (Route != null && !Identity.IsActionAuthorized(Route.Name))
                CreateAccessDeniedException();

            if (this is ICmdsUserControl)
                ApplyAccessControlForCmds();
        }

        public virtual void ApplyAccessControlForCmds()
        {
            if (Identity.IsOperator)
            {
                Access = Access.SetAll(true);
            }
            else
            {
                var permissionId = Route.ToolkitNumber;

                Access = Access.SetAdministrate(Identity.IsGranted(permissionId, PermissionOperation.Delete), false);
                Access = Access.SetConfigure(Access.Administrate || Identity.IsGranted(permissionId, PermissionOperation.Configure), false);
                Access = Access.SetDelete(false, false);
                Access = Access.SetWrite(Access.Administrate || Identity.IsGranted(permissionId, PermissionOperation.Write), false);
                Access = Access.SetRead(Access.Write || Identity.IsGranted(permissionId, PermissionOperation.Read));

                var requestedUserId = User.UserIdentifier;

                if (Guid.TryParse(Request["userID"], out var userId))
                    requestedUserId = userId;

                if (!IsCmdsLearnerVisibleToAdministrator(requestedUserId))
                    Access = Access.SetAll(false);
            }
        }

        protected void CreateAccessDeniedException()
            => HttpResponseHelper.SendHttp403();

        private bool IsCmdsLearnerVisibleToAdministrator(Guid learner)
        {
            var access = Access;

            var currentUser = User.UserIdentifier;
            var organizationId = CurrentIdentityFactory.ActiveOrganizationIdentifier;

            var hasLevel5 = Access.Configure;

            var isPersonInCompany = PersonSearch.IsUserAssignedToOrganization(learner, organizationId);

            // Visibility Rules (X = current user, Y = target person):
            //   Level 5 = X has access to everyone (Y is always visible)
            //   Level 4 = X has access to everyone in the current organization (Y is visible if Y is assigned to the current organization)
            //   Level 3 = X has access to direct reports (Y is visible if Y reports to X)

            // Return True if the current user is the target person OR the current user has Level 5 access.

            if (learner == currentUser || hasLevel5)
            {
                access.Read = true;
                access.Create = true;
                access.Write = true;
                access.Delete = true;
                Access = access;
                return true;
            }

            // Return false if the target person is not in the active company.

            if (!isPersonInCompany)
            {
                access.Read = false;
                access.Create = false;
                access.Write = false;
                access.Delete = false;
                Access = access;
                return false;
            }

            // Return true if you have Level 4 access.	

            if (Access.Administrate)
            {
                access.Read = true;
                access.Create = true;
                access.Write = true;
                access.Delete = true;
                Access = access;
                return true;
            }

            // At this point in the code we know the target person is assigned to the active company, and we know that you have Level 2	
            // or 3 access. If there is a relationship from you to the target person then return true.	

            var fromContactID = currentUser;
            var toContactID = learner;
            var count = UserConnectionSearch.Count(fromContactID, toContactID);

            access.Read = count > 0;
            access.Create = false;
            access.Write = false;
            access.Delete = false;
            Access = access;
            return false;
        }

        private void RequireAuthentication()
        {
            var signout = SignOut.GetUrl();

            if (IsAuthorizationRequirementSatisfied())
                return;

            if (!AllowUnauthenticatedAccess())
                return;

            var rawUrl = HttpContext.Current.Request.RawUrl;
            var isAllow = AllowUnauthenticatedUsers(rawUrl)
                || rawUrl.Equals(signout, StringComparison.OrdinalIgnoreCase)
                || rawUrl.StartsWith("/lti/", StringComparison.OrdinalIgnoreCase);

            if (isAllow)
                return;

            if (IsAuthenticationRequired)
                RequireAuthenticationHelper();

            if (!Request.RawUrl.StartsWith("/ui/lobby/")
                && !Request.RawUrl.StartsWith("/ui/portal/workflow/forms/submit/"))
            {
                RedirectToUrl(this, $"/ui/lobby/signin?ReturnUrl={HttpUtility.UrlEncode(Request.RawUrl)}");
                return;
            }
        }

        private bool AllowUnauthenticatedAccess()
        {
            var master = Master as PortalMaster;
            if (master?.IsSalesReady == true && (ActionModel?.AllowUnauthenticatedUsers() ?? false))
                return false;
            return true;
        }

        private void RequireAuthenticationHelper()
        {
            var rawUrl = Request.RawUrl;

            if (AllowUnauthenticatedUsers(rawUrl))
                return;

            var signout = SignOut.GetUrl();
            if (StringHelper.Equals(rawUrl, signout))
                return;

            var lti = "/lti/";
            if (StringHelper.StartsWith(rawUrl, lti))
                return;

            var signoutWebUrl = SignOut.GetWebUrl();
            if (!string.IsNullOrEmpty(rawUrl))
                signoutWebUrl.QueryString.Add("returnurl", HttpUtility.UrlEncode(rawUrl));

            signoutWebUrl.QueryString.Add("error", "Authentication is required for access to this page.");

            RedirectToUrl(this, signoutWebUrl.ToString());
        }

        private bool IsAuthorizationRequirementSatisfied()
        {
            if (Request.IsAuthenticated)
                return true;

            var authRequirement = ActionModel?.AuthorizationRequirement;
            if (authRequirement.IsEmpty())
                return false;

            var authSource = CookieTokenModule.Current.AuthenticationSource.IfNullOrEmpty(AuthenticationSource.None);
            if (authRequirement == ActionAuthorizationRequirement.ShiftIqExamEventAuthentication && authSource == AuthenticationSource.ShiftIqExamEvent)
                return true;

            return false;
        }

        private bool IsMultifactorAuthenticationDisabled()
        {
            return Identity.IsImpersonating
                || ActionModel?.AuthorizationRequirement == ActionAuthorizationRequirement.ShiftIqExamEventAuthentication;
        }

        #endregion

        #region Localization

        protected string LocalizeDate(object time)
        {
            if (time == null)
                return "-";

            var sentinel = Shift.Common.Calendar.UnknownUtc;
            return time is DateTimeOffset && ((DateTimeOffset)time) == sentinel
                ? "-"
                : time != null && time is DateTimeOffset
                ? TimeZones.FormatDateOnly((DateTimeOffset)time, User.TimeZone)
                : string.Empty;
        }

        protected string LocalizeTime(object time, string element = null)
        {
            if (time == null)
                return "-";

            if (time is DateTime dt)
                return dt.Format();

            var t = (DateTimeOffset)time;
            if (t == null)
                return "-";

            if (element != null)
                return $"{t.Format(User.TimeZone, true)} <{element} class='form-text'>{t.Humanize()}</{element}>";

            var sentinel = Shift.Common.Calendar.UnknownUtc;
            return t == sentinel ? "-" : t.Format(User.TimeZone, true, false);
        }

        #endregion

        public void OverrideHomeLink(string url)
        {
            if (Page.Master is PortalMaster master)
                master.OverrideHomeLink(url);
        }

        public void RenderBreadcrumb(string urlArgs)
        {

        }
    }
}