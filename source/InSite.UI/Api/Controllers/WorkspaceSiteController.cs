using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

using InSite.Api.Models;
using InSite.Api.Settings;
using InSite.Application.Contacts.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.Web.Data;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Workspace")]
    [RoutePrefix("api/sites")]
    public class SitesController : ApiBaseController
    {
        #region Classes

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class DataModel
        {
            [JsonProperty(PropertyName = "id")]
            public Guid Identifier { get; set; }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "slug")]
            public string Slug { get; set; }

            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "parentType")]
            public string ParentType { get; set; }

            [JsonProperty(PropertyName = "parentTitle")]
            public string ParentTitle { get; set; }
        }

        #endregion

        [AllowAnonymous]
        [HttpGet]
        [Route("{domain}/{*value}")]
        public HttpResponseMessage Pages(string domain, string value)
        {
            var organization = ApiHelper.GetOrganization();

            if (IsSuspiciousInput(value))
            {
                var response = Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    error = "Injection attempt",
                    reason = $"Input validation failed: {value}",
                    code = 1002,
                    ip = GetClientIp(Request)
                });

                response.ReasonPhrase = "AppCode-1002";
                response.Headers.Add("X-App-Code", "1002");

                return response;
            }

            if (int.TryParse(value, out var page) && page == 0)
            {
                var list = GetPageList();
                return JsonSuccess(list);
            }
            else
            {
                var post = GetPageDetail(value);
                if (post == null)
                    return JsonError("Page not found.", HttpStatusCode.BadRequest);

                return JsonSuccess(post);
            }

            BlogEntry GetPageDetail(string name)
            {
                var resource = VWebPageHierarchySearch.GetPage(organization.OrganizationIdentifier, $"{name}");
                if (resource == null)
                    return null;

                var content = ServiceLocator.ContentSearch.GetBlock(
                    resource.WebPageIdentifier,
                    ContentContainer.DefaultLanguage,
                    new[] { ContentLabel.Title, ContentLabel.Body });

                return new BlogEntry
                {
                    Title = content.Title.GetText(),
                    Content = content.Body.GetHtml()
                };
            }

            List<BlogSearchResult> GetPageList()
            {
                var results = new List<BlogSearchResult>();
                var pages = VWebPageHierarchySearch.GetPages(organization.OrganizationIdentifier, domain);
                var blogs = new List<BlogSearchResult>();

                foreach (var p in pages)
                {
                    if (p.WebPageType == "Block")
                        continue;

                    var blog = ServiceLocator.PageSearch.GetPage(p.WebPageIdentifier);
                    if (blog == null)
                        continue;

                    var content = ServiceLocator.ContentSearch.GetBlock(
                        blog.PageIdentifier,
                        ContentContainer.DefaultLanguage,
                        new[] { ContentLabel.Title, ContentLabel.Summary });

                    var b = new BlogSearchResult
                    {
                        Path = p.PathUrl,
                        Name = blog.PageSlug,
                        Day = blog.AuthorDate.HasValue ? blog.AuthorDate.Value.ToString("dd") : string.Empty,
                        Month = blog.AuthorDate.HasValue ? blog.AuthorDate.Value.ToString("MMM yyyy") : string.Empty,
                        Date = blog.AuthorDate ?? blog.LastChangeTime ?? DateTimeOffset.Now,
                        Title = content.Title.Text.Default,
                        Summary = content.Summary.Text.Default
                    };

                    blogs.Add(b);
                }

                return blogs.OrderByDescending(b => b.Date).ToList();
            }
        }

        private static bool IsSuspiciousInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            return !Regex.IsMatch(input, @"^[a-zA-Z0-9/\-]+$");
        }

        private static string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop?.Address;
            }

            return "Unknown";
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("contact")]
        public HttpResponseMessage Contact(PagesContactUsModel model)
        {
            var organization = ApiHelper.GetOrganization();

            var address = GetClientIpAddress(Request);

            var alert = new AlertHelpRequested
            {
                BrowserAddress = address,
                Organization = organization.LegalName,
                RequestDescription = model.Message,
                RequestSource = "InSite.Api.Controllers.SitesController",
                RequestSummary = model.Subject,
                UserEmail = model.Email,
                UserName = model.Name,
                UserPhone = model.Phone,
            };

            ServiceLocator.AlertMailer.Send(organization.Identifier, Shift.Constant.UserIdentifiers.Someone, alert);

            return JsonSuccess("Thanks for contacting us. We'll respond by email or telephone as soon as possible.");
        }

        [HttpGet]
        [Route("getnavigationmenu")]
        public HttpResponseMessage GetNavigationMenu(Guid site)
        {
            var root = Domain.Foundations.Navigation.NavigationMenu.Create(ServiceLocator.PageSearch.CreateTree(site));
            return JsonSuccess(root);
        }

        [HttpGet]
        [Route("getuser")]
        public HttpResponseMessage GetUser()
        {
            var origin = "*";
            PagesUserInfo user = null;

            if (Request.Headers.Contains("Origin"))
            {
                var requestOrigin = Request.Headers.GetValues("Origin").First();
                if (!string.IsNullOrEmpty(requestOrigin) && requestOrigin.EndsWith($".{ServiceLocator.AppSettings.Security.Domain}"))
                {
                    origin = requestOrigin;
                    user = PagesUserInfo.Get(CurrentOrganization.Identifier);
                }
            }

            var response = user != null ? JsonSuccess(user) : JsonSuccess("User not found", HttpStatusCode.NotFound);

            response.Headers.Add("Access-Control-Allow-Origin", origin);
            response.Headers.Add("Access-Control-Allow-Credentials", "true");

            return response;
        }

        /// <summary>
        /// Rename a site
        /// </summary>
        /// <remarks>
        /// Modify the domain name for an existing website.
        /// </remarks>
        [HttpPost]
        [Route("rename")]
        public HttpResponseMessage Rename([FromBody] RenameRequest rename)
        {
            var fromWithSubdomain = "." + rename.From;

            var sites = ServiceLocator.SiteSearch
                .Bind(
                    x => new
                    {
                        x.SiteIdentifier,
                        x.SiteDomain
                    },
                    x => x.SiteDomain == rename.From || x.SiteDomain.EndsWith(fromWithSubdomain)
                );

            var renamed = new List<(Guid siteId, string from, string to)>();
            foreach (var site in sites)
            {
                var domain = site.SiteDomain.Length == rename.From.Length
                    ? rename.To
                    : site.SiteDomain.Substring(0, site.SiteDomain.Length - rename.From.Length) + rename.To;

                renamed.Add((site.SiteIdentifier, site.SiteDomain, domain));
            }

            try
            {
                foreach (var site in renamed)
                    SendCommand(new ChangeSiteDomain(site.siteId, site.to));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message, HttpStatusCode.InternalServerError);
            }

            var result = renamed.Select(x => new
            {
                x.from,
                x.to
            })
            .ToList();

            return JsonSuccess(new { count = result.Count, domains = result });
        }

        [HttpPost]
        [Route("sendemail")]
        public HttpResponseMessage SendEmail([FromBody] PagesContactUsModel2 model)
        {
            var organization = ApiHelper.GetOrganization();
            if (organization == null)
                throw new System.Exception("Organization not found");

            var request = HttpContext.Current.Request;
            var address = request.UserHostAddress;
            var browser = request.Browser;

            var alert = new AlertHelpRequested
            {
                BrowserAddress = address,
                BrowserName = $"{browser.Platform} {browser.Browser} {browser.Version}",
                Organization = organization.LegalName,
                RequestDescription = model.Comment,
                RequestSource = "InSite.Api.Controllers.SitesController",
                RequestSummary = "Web Site Help Request",
                RequestUrl = $"{request.Url.Scheme}://{request.Url.Host}{request.RawUrl}",
                UserEmail = model.Email,
                UserName = model.Name,
                UserPhone = model.Phone
            };

            ServiceLocator.AlertMailer.Send(organization.OrganizationIdentifier, Shift.Constant.UserIdentifiers.Someone, alert);

            return JsonSuccess("OK");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("settings")]
        public HttpResponseMessage Settings()
        {
            var organization = ApiHelper.GetOrganization();
            var subdomain = organization.OrganizationCode;
            var domain = ServiceLocator.AppSettings.Security.Domain;
            var json = JsonSuccess(new PagesSettingsModel { LoginPageUrl = $"https://{subdomain}.{domain}{InSite.Web.SignIn.SignInLogic.GetUrl()}" });
            return json;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("blogs/subscribe")]
        public HttpResponseMessage Subscribe(CmdsBlogSubscriptionRequested model)
        {
            var organization = ApiHelper.GetOrganization();

            if (!EmailAddress.IsValidAddress(model.Email))
                return JsonBadRequest($"Invalid email address: {model.Email}");

            var userId = UserSearch.BindFirst(x => (Guid?)x.UserIdentifier, new UserFilter { EmailExact = model.Email });
            if (userId == null)
                return JsonBadRequest("Subscribing to the blog is available to CMDS users only. Please contact admin_cmds@keyera.com for more information.");

            var groupFilter = new QGroupFilter { GroupName = "CMDS Blog Subscribers", OrganizationIdentifier = OrganizationIdentifiers.CMDS };
            var groups = ServiceLocator.GroupSearch.GetGroups(groupFilter);
            if (groups.Count == 0)
                return JsonBadRequest($"Group not found: {groupFilter.GroupName}");

            var group = groups.First();
            MembershipHelper.Save(group.GroupIdentifier, userId.Value, "Membership");

            var messageFilter = new MessageFilter
            {
                Type = MessageTypeName.Newsletter,
                OrganizationIdentifier = OrganizationIdentifiers.CMDS,
                Name = "CMDS News Update",
            };
            var message = ServiceLocator.MessageSearch.GetMessage(messageFilter);
            if (message == null)
                return JsonBadRequest($"Message not found: {messageFilter.Name}");

            ServiceLocator.SendCommand(new AddSubscriber(message.MessageIdentifier, userId.Value, "Message Recipient", false, false));

            return JsonSuccess("Blog subscription request completed successfully.");
        }

        private string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;

            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                return ((RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name]).Address;

            return "Unavailable";
        }

        [HttpGet]
        [Route("pages/list")]
        public HttpResponseMessage List(string site, string type, string keyword, int start, int end)
        {
            var filter = new QPageFilter
            {
                OrganizationIdentifier = CurrentOrganization.OrganizationIdentifier
            };

            if (Guid.TryParse(site, out var siteId))
                filter.WebSiteIdentifier = siteId;
            else
                filter.WebSiteAssigned = false;

            if (!string.IsNullOrEmpty(type))
                filter.Types.Add(type);

            if (!string.IsNullOrEmpty(keyword))
                filter.Keyword = keyword;

            filter.Paging = Paging.SetStartEnd(start, end);
            filter.OrderBy = "Title";

            var result = ServiceLocator.PageSearch.Bind(
                x => new DataModel
                {
                    Identifier = x.PageIdentifier,
                    Type = x.PageType,
                    Slug = x.PageSlug,
                    Title = x.Title,
                    ParentType = x.Parent.PageType,
                    ParentTitle = x.Parent.Title,
                },
                filter);

            return JsonSuccess(result);
        }
    }

    public class RenameRequest
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}