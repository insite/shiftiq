using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Contacts.Read;
using InSite.Application.Sites.Read;
using InSite.Domain.Foundations;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Content
{
    public class LaunchCardAdapter
    {
        private static string BuildUrl(string path, string parameters)
        {
            return parameters.IsNotEmpty()
                ? path.Contains("?") ? path + "&" + parameters : path + "?" + parameters
                : path;
        }

        public string CreateUrl(string appUrl, Guid pageId, string pageUrl, string launchType, Guid? launchId, string launchSlug, Guid? learner, string caller, Func<string, string> translate)
        {
            if (launchType == "Survey")
            {
                var search = new SurveySearch(null);

                var survey = launchId.HasValue
                    ? search.GetSurveyForm(launchId.Value)
                    : null;

                var url = survey != null && learner != null
                    ? StartSurvey(appUrl, survey.AssetNumber, learner)
                    : null;

                if (url.IsNotEmpty() && caller.IsNotEmpty())
                    return BuildUrl(url, $"caller={caller}");

                return HttpUtility.HtmlEncode($"javascript:alert('{translate("The survey is not found")}.')");
            }
            else if (launchType == "Program")
            {
                var program = launchType == "Program" && launchId.HasValue
                    ? ProgramSearch1.SelectFirst(x => x.ProgramIdentifier == launchId)
                    : null;

                var url = program != null
                    ? $"{appUrl}/ui/portal/learning/program/{launchSlug}"
                    : null;

                if (url.IsNotEmpty())
                    return url;

                return HttpUtility.HtmlEncode($"javascript:alert('{translate("The program is not found")}.')");
            }

            return pageUrl.IsNotEmpty()
                ? pageUrl
                : new PageSearch(null, null).GetPagePath(pageId, false);
        }

        private string CreateUrl(string appUrl, QPage page, string portalName, ISecurityFramework identity, string caller, Func<string, string> translate)
        {
            if (page.ContentControl == "Survey")
            {
                var surveySearch = new SurveySearch(null);

                var survey = page.ObjectType == "Survey" && page.ObjectIdentifier.HasValue
                    ? surveySearch.GetSurveyForm(page.ObjectIdentifier.Value)
                    : null;

                var url = survey != null && identity != null
                    ? StartSurvey(appUrl, survey.AssetNumber, identity.User.Identifier)
                    : null;

                if (url.IsNotEmpty())
                    return BuildUrl(url, $"caller={caller}");

                return HttpUtility.HtmlEncode($"javascript:alert('{translate("The survey is not found")}.')");
            }
            else if (page.ContentControl == "Program")
            {
                var currentUrl = HttpContext.Current.Request.Url;

                var survey = page.ObjectType == "Program" && page.ObjectIdentifier.HasValue
                    ? ProgramSearch1.SelectFirst(x => x.ProgramIdentifier == page.ObjectIdentifier)
                    : null;

                var url = survey != null
                    ? $"{currentUrl.Scheme}://{currentUrl.Host}/ui/portal/learning/program/{page.PageSlug}"
                    : null;

                if (url.IsNotEmpty())
                    return url;

                return HttpUtility.HtmlEncode($"javascript:alert('{translate("The program is not found")}.')");
            }

            var pageSearch = new PageSearch(null, null);

            return !string.IsNullOrWhiteSpace(page.NavigateUrl)
                ? page.NavigateUrl
                : pageSearch.GetPagePath(page.PageIdentifier, false);
        }

        public string ResumeSurvey(Guid session)
            => $"/ui/portal/workflow/forms/submit/resume?session={session}";

        public string ReviewSurvey(Guid session)
            => $"/ui/portal/workflow/forms/submit/review?session={session}";

        public string StartSurvey(string appUrl, int form, Guid? user = null, string language = null)
        {
            var surveyUrl = $"{appUrl}/form/{form}";

            if (user != null)
            {
                if (user == Guid.Empty)
                    surveyUrl += "/##RecipientIdentifier##";
                else
                    surveyUrl += "/" + user.ToString();
            }

            if (language.IsNotEmpty())
                surveyUrl += "?language=" + language;

            return surveyUrl;
        }

        public List<LaunchCard> GetPortals(Guid? siteId, ISecurityFramework identity)
        {
            var portals = new List<LaunchCard>();

            if (siteId == null)
                return portals;

            var search = new PageSearch(null, null);

            var pages = search
                .Bind(
                    x => x,
                    x => x.SiteIdentifier == siteId
                        && x.ParentPageIdentifier == null)
                .OrderBy(x => x.Sequence).ThenBy(x => x.Title)
                .ToList();

            var containers = pages.Select(x => x.PageIdentifier).ToList();

            var permissions = TGroupPermissionSearch.Select(x => containers.Any(y => y == x.ObjectIdentifier)).ToList();

            TGroupPermissionSearch.SetIsAccessDenied(pages, identity, permissions);

            foreach (var page in pages)
            {
                if (!page.IsAccessDenied && !page.IsHidden)
                    portals.Add(new LaunchCardAdapter().ToCard(page, identity.Language));
            }

            return portals;
        }

        public List<LaunchCard> GetAssessmentCards(ISecurityFramework identity, Func<Guid, string> assessmentStartUrl)
        {
            var pageSearch = new PageSearch(null, null);

            var pages = pageSearch.Bind(
                    x => x,
                    x => x.OrganizationIdentifier == identity.Organization.OrganizationIdentifier
                      && x.ContentControl == "Assessment"
                      && !x.IsHidden
                      && x.ObjectIdentifier != null);

            var accessibleObjects = TGroupPermissionSearch.GetObjectsAccessibleToUser(identity.User.UserIdentifier);

            var identifiers = pages.Select(x => x.PageIdentifier).ToList();
            var permissions = TGroupPermissionSearch.Bind(x => x, x => identifiers.Any(i => i == x.ObjectIdentifier)).ToList();

            var isSiteAdministrator = identity.IsOperator || identity.IsGranted(PermissionIdentifiers.Admin_Sites, PermissionOperation.Administrate);

            pages = pages
                .Where(x => isSiteAdministrator || IsAccessible(x.PageIdentifier, x.OrganizationIdentifier, permissions, accessibleObjects, identity.Organization.Identifier))
                .ToArray();

            var cards = pages
                .Select(x => new LaunchCard
                {
                    Identifier = x.PageIdentifier,
                    Url = x.NavigateUrl,
                    Title = x.Title,
                    Icon = x.PageIcon,
                    Sequence = x.Sequence
                })
                .OrderBy(x => x.Sequence).ThenBy(x => x.Title)
                .ToList();

            foreach (var card in cards)
            {
                var program = pages.Single(x => x.PageIdentifier == card.Identifier);

                card.Url = assessmentStartUrl(program.ObjectIdentifier.Value);

                var contentSearch = new TContentSearch();

                var content = contentSearch.GetBlock(
                    card.Identifier,
                    labels: new[] { ContentLabel.Title, ContentLabel.Summary });

                var title = content.Title.Text[identity.Language];
                if (title.IsNotEmpty())
                    card.Title = title;

                card.Summary = content.Summary.Text[identity.Language];
            }

            cards = cards
                .Where(x => x.Url != null && !x.Url.StartsWith("/ui/admin/")).ToList();

            return cards;
        }

        public List<LaunchCard> GetLaunchCards(string appUrl, Guid root, string portalName, ISecurityFramework identity, string caller, Func<string, string> translate, Func<string, Guid?, string> progress)
        {
            var pages = new List<LaunchCard>();

            var pageSearch = new PageSearch(null, null);

            var programs = pageSearch
                .Bind(x => x, x => x.ParentPageIdentifier == root && !x.IsHidden)
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Title)
                .ToList();

            var containers = programs.Select(x => x.PageIdentifier).ToList();
            containers.AddRange(programs.Where(x => x.ObjectType == "Program" && x.ObjectIdentifier != null).Select(x => x.ObjectIdentifier.Value));

            var permissions = TGroupPermissionSearch.Select(x => containers.Any(y => y == x.ObjectIdentifier)).ToList();

            TGroupPermissionSearch.SetIsAccessDenied(programs, identity, permissions);

            foreach (var program in programs)
            {
                if (program.IsAccessDenied || program.IsHidden)
                    continue;

                if (program.ObjectIdentifier.HasValue && program.ObjectType == "Course" && TGroupPermissionSearch.IsAccessDenied(program.ObjectIdentifier.Value, identity))
                    continue;

                if (program.ObjectIdentifier.HasValue && program.ObjectType == "Program" && TGroupPermissionSearch.IsAccessDenied(program.ObjectIdentifier.Value, identity))
                    continue;

                pages.Add(GetLaunchCard(appUrl, program, portalName, identity, caller, translate, progress));
            }

            return pages;
        }

        private LaunchCard GetLaunchCard(string appUrl, QPage page, string portalName, ISecurityFramework identity, string caller, Func<string, string> translate, Func<string, Guid?, string> progress)
        {
            var contentSearch = new TContentSearch();

            var isSurvey = page.ContentControl == "Survey";
            var content = contentSearch.GetBlock(
                page.PageIdentifier,
                identity.Language,
                new[] { ContentLabel.Title, ContentLabel.Summary, ContentLabel.ImageUrl });

            var item = new LaunchCard
            {
                Title = content.Title.GetText(identity.Language),
                Summary = content.Summary.GetText(identity.Language),
                Target = page.IsNewTab ? "_blank" : "_self"
            };

            if (page.PageIcon.IsNotEmpty())
                item.Icon = page.PageIcon;
            else if (isSurvey)
                item.Icon = "far fa-comment-check";
            else
                item.Icon = page.PageType == "Folder" ? "far fa-folder" : "far fa-file-alt";

            if (string.IsNullOrEmpty(item.Title))
                item.Title = page.Title;

            item.Url = CreateUrl(appUrl, page, portalName, identity, caller, translate);

            if (content.Exists(ContentLabel.ImageUrl))
                item.Image = content.GetText(ContentLabel.ImageUrl, identity.Language);

            item.Flag = new Flag
            {
                Color = "Info",
                Text = progress(page.Hook, page.ObjectIdentifier)
            };

            return item;
        }

        private bool IsAccessible(Guid assetId, Guid assetOrganizationId, List<TGroupPermission> permissions, ICollection<Guid> userAssets, Guid userOrganizationId)
        {
            if (!permissions.Any(p => p.ObjectIdentifier == assetId))
                return assetOrganizationId == userOrganizationId;

            return userAssets != null && userAssets.Contains(assetId);
        }

        public LaunchCard ToCard(QPage page, string language)
        {
            var card = new LaunchCard();

            var content = TContentSearch.Instance.GetBlock(
                page.PageIdentifier,
                language,
                new[] { ContentLabel.Title, ContentLabel.Body });

            card.Title = (content.Title?.GetText(language)).IfNullOrEmpty(page.Title);
            card.Icon = page.PageIcon;
            card.Identifier = page.PageIdentifier;
            card.Slug = page.PageSlug;

            card.BodyHtml = content.Body.Html[language];
            if (card.BodyHtml.IsEmpty())
                card.BodyHtml = Markdown.ToHtml(content.Body.Text[language]);

            return card;
        }
    }
}