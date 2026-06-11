using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web;
using InSite.Domain.Foundations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Integration.Partitions;
using Shift.Constant;
using Shift.Toolbox;

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

        private class PartitionCard
        {
            public string Brand { get; set; }
            public string Domain { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public string Slug { get; set; }
            public string Theme { get; set; }
            public string LogoUrl { get; set; }

            public string Heading
            {
                get { return _heading ?? $"Partition #{Number}: {Name}"; }
                set { _heading = value; }
            }
            private string _heading;

            public List<OrganizationLink> Organizations { get; set; } = new List<OrganizationLink>();
        }

        private class OrganizationLink
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string LogoUrl { get; set; }
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

            var online = TryGetPartitions(out var partitions);

            if (!online)
            {
                // Engine API offline: warn and keep the existing organization selection.
                ApiOfflineAlert.Visible = true;
                ShowOrganizationSelection();
            }
            else if (partitions.Count == 0)
            {
                // Online but no partitions: warn and keep the existing organization selection.
                NoPartitionsAlert.Visible = true;
                ShowOrganizationSelection();
            }
            else
            {
                // No warnings: render the partition cards instead of the selection.
                OrganizationPanel.Visible = false;
                PartitionsPanel.Visible = true;
                BindPartitions(partitions);
            }

            LoadRecentSessions();
        }

        private void ShowOrganizationSelection()
        {
            var organizations = SelectOrganizations();

            if (AutoRedirect)
                AutoSelectOrganization(organizations);

            OrganizationRepeater.DataSource = organizations;
            OrganizationRepeater.DataBind();
        }

        private bool TryGetPartitions(out List<PartitionRegistration> partitions)
        {
            try
            {
                var client = new PartitionClient(ServiceLocator.AppSettings.Engine);
                partitions = client.GetPartitions();
                return true;
            }
            catch (Exception ex)
            {
                AppSentry.SentryWarning(ex);
                partitions = new List<PartitionRegistration>();
                return false;
            }
        }

        private void BindPartitions(List<PartitionRegistration> partitions)
        {
            var cards = partitions
                .Select(p => new PartitionCard
                {
                    Brand = p.Brand,
                    Domain = p.Domain,
                    Number = p.Number,
                    Name = p.Name,
                    Slug = p.Slug.ToUpper(),
                    Theme = p.Theme,
                    LogoUrl = p.LogoUrl,

                    Organizations = p.Organizations
                        .Where(o => !IsClosed(o.Account))
                        .Where(x => AllowSelection(CurrentSessionState.Identity, x.Identifier))
                        .Where(x => !x.Slug.StartsWith("unit-test-orgcode"))
                        .Select(o => new OrganizationLink
                        {
                            Name = o.Name,
                            Url = GetTenantUrl(p.Domain, o.Slug),
                            LogoUrl = o.LogoUrl
                        })
                        .OrderBy(o => o.Name)
                        .ToList()
                })
                .Where(c => c.Organizations.Count > 0)
                .ToList();

            ResolveLogos(cards);

            if (cards.Count == 1)
            {
                var card = cards.Single();
                card.Heading = card.Domain;
            }

            PartitionRepeater.DataSource = cards;
            PartitionRepeater.DataBind();
        }

        // Default logo served from the local app, so it can never 404. Matches WallpaperManager.

        private static string DefaultLogoUrl =>
            ServiceLocator.Partition.IsE03() ? "/ui/lobby/images/cmds.png" : "/ui/lobby/images/shift.png";

        // Replace each organization logo with one that is known to resolve, so the browser never
        // requests a URL that 404s. (Repeated 404s from logo <img> tags can trip the IP Ban module
        // against the user.) The fallback order is: the organization's own logo, then its
        // partition's logo, then the default. Checks run in parallel because each is a network HEAD
        // request with its own timeout.

        private static void ResolveLogos(List<PartitionCard> cards)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = 8 };

            // Resolve each partition logo once: it's the shared fallback for that partition's
            // organizations. After this, card.LogoUrl is a validated URL or null.

            Parallel.ForEach(cards, options, card => card.LogoUrl = ResolveExisting(card.LogoUrl));

            var links = cards
                .SelectMany(c => c.Organizations.Select(o => new { Card = c, Organization = o }))
                .ToList();

            Parallel.ForEach(links, options, link =>
            {
                var organizationLogo = ResolveExisting(link.Organization.LogoUrl);

                link.Organization.LogoUrl = organizationLogo ?? link.Card.LogoUrl ?? DefaultLogoUrl;
            });
        }

        // Returns the URL when it points at an image that actually exists, otherwise null.

        private static string ResolveExisting(string logoUrl)
        {
            if (string.IsNullOrEmpty(logoUrl))
                return null;

            var isAbsolute = Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri);

            if (!isAbsolute)
                return null;

            var exists = ImageHelper.Exists(uri);

            return exists ? logoUrl : null;
        }

        private static bool IsClosed(AccountRegistration account)
        {
            return account != null
                && string.Equals(account.Status, "Closed", StringComparison.OrdinalIgnoreCase);
        }

        // Fully qualified tenant URL for an organization, using its partition's own
        // domain plus the current environment's subdomain prefix (e.g. https://local-abc.example.com).
        private string GetTenantUrl(string partitionDomain, string organizationSlug)
        {
            var scheme = Request.Url.Scheme;
            var prefix = ServiceLocator.AppSettings.Environment.GetSubdomainPrefix();

            return $"{scheme}://{prefix}{organizationSlug}.{partitionDomain}";
        }

        private List<DataItem> SelectOrganizations()
        {
            return CurrentSessionState.Identity.Organizations
                .Where(x => x.AccountClosed == null)
                .Where(x => AllowSelection(CurrentSessionState.Identity, x.OrganizationIdentifier))
                .Where(x => !x.OrganizationCode.StartsWith("unit-test-orgcode"))
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
                relativeUrl = Shift.Common.Urls.HomeUrl;

            else if (Identity.IsAdministrator && Identity.IsGranted(PermissionNames.Admin_Home))
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
                    url = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, organization, Urls.HomeUrl);
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
            var hasMultiPartitionAccess = StringHelper.EndsWithAny(identity.User.Email, new[] { "@local", "@shiftiq.com" });

            if (hasMultiPartitionAccess)
                return true;

            var person = identity.Persons.FirstOrDefault(x => x.Organization == organizationId);

            if (person == null)
                return false;

            var ok = person.IsAdministrator || person.IsLearner || ServiceLocator.Partition.IsE03();

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
                ? Urls.HomeUrl
                : RelativeUrl.AdminHomeUrl;

            var link = (HyperLink)e.Item.FindControl("StartLink");
            link.NavigateUrl = PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, item.OrganizationCode, url);
            link.Text = item.CompanyTitle;
        }

        #endregion
    }
}
