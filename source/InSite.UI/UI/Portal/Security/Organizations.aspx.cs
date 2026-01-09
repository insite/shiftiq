using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Security
{
    public partial class Organizations : Layout.Portal.PortalBasePage
    {
        #region Classes

        private class DataItem
        {
            public string OrganizationCode { get; set; }
            public string CompanyTitle { get; set; }
            public string RedirectUrl { get; set; }
        }

        #endregion

        #region Properties

        private bool IsLogin => Request["login"] == "1";

        private bool AutoRedirect => Request["auto-redirect"] != "0";

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SessionRepeater.ItemDataBound += SessionRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var count = CurrentSessionState.Identity.Organizations.Count;

            if (count == 0)
            {
                CurrentIdentityFactory.SignedOut();

                var reason = $"{User.Email} must be assigned to at least one organization before authentication is permitted";

                Lobby.SignOut.Redirect(this, reason);

                throw ApplicationError.Create(reason);
            }

            PageHelper.AutoBindHeader(this);

            var organizations = SelectOrganizations();

            if (AutoRedirect)
                AutoSelectOrganization(organizations);

            OrganizationRepeater.DataSource = organizations;
            OrganizationRepeater.DataBind();

            LoadRecentSessions();
        }

        private List<DataItem> SelectOrganizations()
        {
            return CurrentSessionState.Identity.Organizations
                .Where(x => x.AccountClosed == null && AllowSelection(CurrentSessionState.Identity, x.OrganizationIdentifier))
                .Select(x => new DataItem
                {
                    OrganizationCode = x.OrganizationCode,
                    CompanyTitle = x.CompanyDescription.LegalName ?? x.CompanyName,
                    RedirectUrl = GetAbsoluteUrl(x.OrganizationCode)
                })
                .OrderBy(x => x.CompanyTitle)
                .ToList();
        }

        private string GetAbsoluteUrl(string organizationCode)
        {
            var relativeUrl = RelativeUrl.PortalHomeUrl;

            if (ServiceLocator.Partition.IsE03() && User.AccessGrantedToCmds)
                relativeUrl = Shift.Common.Urls.CmdsHomeUrl;

            else if (Identity.IsAdministrator && Identity.IsActionAuthorized(RelativeUrl.AdminHomeUrl))
                relativeUrl = RelativeUrl.AdminHomeUrl;

            return PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organizationCode, relativeUrl);
        }

        private void LoadRecentSessions()
        {
            var recentSessions = GetRecentSessions();
            SessionRepeater.DataSource = recentSessions;
            SessionRepeater.DataBind();
        }

        private void AutoSelectOrganization(List<DataItem> organizations)
        {
            string url = null;

            if (ServiceLocator.Partition.IsE03())
            {
                if (organizations.Count == 1)
                {
                    var organization = organizations.FirstOrDefault().OrganizationCode;
                    url = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organization, Urls.CmdsHomeUrl);
                }
            }
            else if (IsLogin)
            {
                var subdomain = CookieTokenModule.Current.OrganizationCode;
                if (organizations.Any(t => t.OrganizationCode == subdomain))
                    url = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, subdomain, ServiceLocator.Urls.GetHomeUrl(Identity.User.AccessGrantedToCmds, ServiceLocator.Partition.IsE03(), Identity.IsAdministrator));
            }
            else if (organizations.Count == 1)
            {
                var organization = organizations.Single();
                url = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organization.OrganizationCode, ServiceLocator.Urls.GetHomeUrl(Identity.User.AccessGrantedToCmds, ServiceLocator.Partition.IsE03(), Identity.IsAdministrator));
            }

            if (url != null)
                HttpResponseHelper.Redirect(url);
        }

        #endregion

        #region Helper methods

        private static bool AllowSelection(ISecurityFramework identity, Guid organizationId)
        {
            var person = identity.Persons.Single(x => x.Organization == organizationId);
            bool ok = person.IsAdministrator || person.IsLearner || ServiceLocator.Partition.IsE03();

            if (ok && identity.IsImpersonating && identity.Impersonator.Organizations != null)
                ok = ok && identity.Impersonator.Organizations.Any(x => x.OrganizationIdentifier == organizationId);

            return ok;
        }

        private static List<TUserSessionCacheSummary> GetRecentSessions()
        {
            var userID = CurrentSessionState.Identity.User.UserIdentifier;
            var recentSessions = OrganizationHelper.GetRecentSessions(userID, 10);

            if (recentSessions.Count == 0)
            {
                SessionHelper.StartSession(CurrentSessionState.Identity.Organization.Identifier, userID);

                RecentSessionHelper.Clear();

                recentSessions = OrganizationHelper.GetRecentSessions(userID, 10);
            }

            return recentSessions;
        }

        protected string GetTimestampHtml(object when)
        {
            var lastSessionStarted = Translate("session started");
            return $"{lastSessionStarted} " + ((DateTimeOffset)when).Humanize(null, LanguageCulture);
        }

        private void SessionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (TUserSessionCacheSummary)e.Item.DataItem;

            var url = ServiceLocator.Partition.IsE03()
                ? Urls.CmdsHomeUrl
                : "/ui/admin/home";

            var link = (HyperLink)e.Item.FindControl("StartLink");
            link.NavigateUrl = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, item.OrganizationCode, url);
            link.Text = item.CompanyTitle;
        }

        #endregion
    }
}