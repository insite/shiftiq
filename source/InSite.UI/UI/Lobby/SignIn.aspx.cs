using System;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Common;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Lobby.Controls;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby
{
    public partial class SignIn : SignInBasePage
    {
        private static SecuritySettings SecuritySettings => ServiceLocator.AppSettings.Security;
        private static ReleaseSettings ReleaseSettings => ServiceLocator.AppSettings.Release;

        #region Properties

        private string RegistrationGroup => Page.Request.QueryString["group"]?.ToString();
        private string ReturnVerifiedUrl => Page.Request.QueryString["returnVerified"]?.ToString();
        private bool AntiForgeryValidationFailed => Page.Request.QueryString["afv_failed"] == "1";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ClearSessionCache();

            if (!IsPostBack)
                RemoveSsrsReportsFromSession();

            if (Context.User != null && Context.User.Identity.IsAuthenticated)
            {
                var email = Context.User.Identity.Name;

                var user = UserSearch.SelectByEmail(email, x => x.Memberships.Select(y => y.Group));

                if (user != null && ReturnVerifiedUrl.HasValue())
                {
                    RedirectURL = RedirectReturnVerifiedUrl(user, ReturnVerifiedUrl);
                    HasExternalReturnUrl = RedirectURL.HasValue();
                }

                if (user != null)
                    RedirectToSignInSucceedPage(false, user.UserIdentifier);
                else
                    SignOut.Redirect(this, $"Authentication succeeded but user email has no matching account: {email}");
            }

            SignInButton.Click += SignInButton_Click;
            MSSignInButton.Click += MSSignInButton_Click;
            GoogleSignInButton.Click += GoogleSignInButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ResetPasswordAnchor.HRef = ResetPassword.GetUrl();

            if (!IsPostBack)
            {
                Title = LabelHelper.GetTranslation(ActionModel.ActionName);

                SetupSignInView();
                SetupCustomContent();

                PasswordExpiredToast.Visible = Request.QueryString["password-expired"] == "1";
            }

            if (string.IsNullOrWhiteSpace(RedirectURL))
            {
                RedirectURL = HttpContext.Current.Request.QueryString["ReturnUrl"];
            }

            if (AntiForgeryValidationFailed)
                SignInStatus.AddMessage(AlertType.Warning, "Your browser triggered an anti-forgery token validation warning. Please sign in again.");
        }

        private void RemoveSsrsReportsFromSession()
        {
            for (var i = Session.Count - 1; i >= 0; i--)
            {
                try
                {
                    var itemType = Session[i]?.GetType().ToString();
                    if (itemType != null && itemType.StartsWith("Microsoft.Reporting.WebForms."))
                        Session.RemoveAt(i);
                }
                catch
                {
                    Session.RemoveAt(i);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            SignInUserName.Focus();

            base.OnPreRender(e);
        }

        #endregion

        #region Methods (views)

        private void SetupSignInView()
        {
            var browser = Request.Browser;
            var showRegistration = false;

            var allowGoogleSignIn = false;
            var allowMicrosoftSignIn = false;

            if (Organization != null)
            {
                showRegistration = Register.IsRegistrationEnabled(Organization, RegistrationGroup);
                allowGoogleSignIn = Organization.PlatformCustomization.SignIn.AllowGoogleSignIn;
                allowMicrosoftSignIn = Organization.PlatformCustomization.SignIn.AllowMicrosoftSignIn;
            }

            CurrentSessionState.EnableUserRegistration = showRegistration;

            if (CurrentSessionState.EnableUserRegistration)
                RegisterLink.HRef = "/ui/lobby/register" + (RegistrationGroup.IsNotEmpty() ? $"?group={RegistrationGroup}" : string.Empty);

            SignInRegistration.Visible = showRegistration;
            SignInIeWarning.Visible = HttpRequestHelper.IsIE;
            SignInBrowserRecommendation.Visible = !StringHelper.EqualsAny(browser.Browser, new[] { "Chrome", "Firefox" });
            SignInBrowser.InnerText = $"Browser: {browser.Browser} {browser.Version}";
            SignInTerms.Visible = !ServiceLocator.Partition.IsE03();
            SignInCommands.Visible = true;
            SignInError.Visible = false;

            SignInPanel.Visible = !HttpRequestHelper.IsIE;

            SignInButtonAlternatives.Visible = allowGoogleSignIn || allowMicrosoftSignIn;
            GoogleSignInButton.Visible = allowGoogleSignIn;
            MSSignInButton.Visible = allowMicrosoftSignIn;

            if (Request.QueryString["referrer-reason"] == "forced-organization")
            {
                SignInReferrer.InnerHtml = "<strong>Please notice</strong> you have been redirected to the login page for your organization account.";
                SignInReferrer.Visible = true;
            }

            EnvironmentLinks.InnerHtml = CreateEnvironmentLinks();

            ConfigureAutologin();
        }

        private void ConfigureAutologin()
        {
            if (!ReleaseSettings.GetEnvironment().IsLocal())
                return;

            var autologin = SecuritySettings.Autologin;

            if (autologin.IsEmpty())
                return;

            var parts = autologin.Split('/');

            if (parts.Length > 0)
            {
                SignInUserName.Text = parts[0];
            }

            if (parts.Length > 1)
            {
                SignInPassword.Text = parts[1];
                SignInPassword.TextMode = TextBoxMode.SingleLine;
            }
        }

        private string CreateEnvironmentLinks()
        {
            var domain = SecuritySettings.Domain;

            var cmds = ServiceLocator.Partition.IsE03();

            var code = UrlHelper.GetOrganizationCode(Page.Request.Url);

            var developmentName = cmds ? "Test" : "Development";
            var developmentUrl = UrlHelper.GetAbsoluteUrl(domain, EnvironmentName.Development, "/", code);

            var sandboxName = cmds ? "Demo" : "Sandbox";
            var sandboxUrl = UrlHelper.GetAbsoluteUrl(domain, EnvironmentName.Sandbox, "/", code);

            var productionName = cmds ? "Live" : "Production";
            var productionUrl = UrlHelper.GetAbsoluteUrl(domain, EnvironmentName.Production, "/", code);

            var environment = ReleaseSettings.GetEnvironment();

            var links = string.Empty;

            if (environment.Name != EnvironmentName.Development)
                links += $"<a href='{developmentUrl}' class='me-2'>{developmentName}</a>";

            if (environment.Name != EnvironmentName.Sandbox)
                links += $"<a href='{sandboxUrl}' class='me-2'>{sandboxName}</a>";

            if (environment.Name != EnvironmentName.Production)
                links += $"<a href='{productionUrl}' class='me-2'>{productionName}</a>";

            var color = environment.Color;

            var html = $"<div class='alert alert-{color}'><p>You are signing in to the <strong>{environment.Name}</strong> environment.</p></div>";

            if (links.Length > 0)
                html += $"<p><span class='me-2'>Other environments:</span>{links}</p>";

            return html;
        }

        #endregion

        #region Event handlers

        private void SetupCustomContent()
        {
            CustomContentCard.Visible = false;

            var slug = Request.Url.AbsolutePath.Substring(1).Replace("/", "-");

            var pages = ServiceLocator.PageSearch.Bind(
                x => x.PageIdentifier,
                x => x.OrganizationIdentifier == Organization.Identifier && x.PageSlug == slug);

            if (pages.Length != 1)
                return;

            var contents = ServiceLocator.ContentSearch.SelectContainerByLabel(pages[0], "Body");
            var content = contents.FirstOrDefault(x => x.ContentLanguage == CurrentLanguage);
            if (content?.ContentText == null)
                return;

            CustomContentCard.Visible = true;
            CustomContentHtml.Text = Markdown.ToHtml(content.ContentText);
        }

        private void SignInButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var username = SignInUserName.Text.Trim();
            var password = SignInPassword.Text;

            ValidateLoginOrganizationCode(username, CookieTokenModule.Current.OrganizationCode);

            var lockedUntil = AccountHelper.SignInLockedUntil;
            if (lockedUntil.HasValue)
                OnBruteForceDetected(username, lockedUntil.Value);
            else
                LoginUser(username, password, false, Shift.Constant.AuthenticationSource.ShiftIq, ReturnVerifiedUrl);
        }

        private void ValidateLoginOrganizationCode(string email, string organization)
        {
            var code = UserSearch.BindFirst(x => x.LoginOrganizationCode, new UserFilter { EmailExact = email });

            if (code == null || StringHelper.Equals(code, organization))
                return;

            var url = ServiceLocator.Urls.GetApplicationUrl(code) + "/ui/lobby/signin?referrer-reason=forced-organization";
            HttpResponseHelper.Redirect(url);
        }

        private void GoogleSignInButton_Click(object sender, EventArgs e)
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var organizationId = Organization.OrganizationIdentifier;
            var link = Global.GoogleLogin.CreateAuthorizationRequest(organizationId, baseUrl, Page.Request.Url.Host);
            HttpResponseHelper.Redirect(link, true);
        }

        private void MSSignInButton_Click(object sender, EventArgs e)
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var organizationId = Organization.OrganizationIdentifier;
            var link = Global.AzureAD.CreateAuthorizationRequest(organizationId, Page.Request.Url.Host, baseUrl);
            HttpResponseHelper.Redirect(link, true);
        }

        #endregion
    }
}