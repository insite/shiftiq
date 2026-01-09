using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Domain.Sites.Pages;

using Shift.Common;

namespace InSite.Application.Sites.Write
{
    public class PageCommandGenerator
    {
        /// <summary>
        /// Returns the list of commands to create a Page.
        /// </summary>
        public List<ICommand> GetCommands(PageState page)
        {
            var script = new List<ICommand>
            {
                new CreatePage(page.Identifier, page.Site, page.ParentPage , page.Tenant, page.Author, page.Title, page.Type, page.Sequence, page.IsHidden, page.IsNewTab)
            };

            if (!string.IsNullOrEmpty(page.Slug))
                script.Add(new ChangePageSlug(page.Identifier, page.Slug));

            if (!string.IsNullOrEmpty(page.NavigateUrl))
                script.Add(new ChangePageNavigationUrl(page.Identifier, page.NavigateUrl));

            if (!string.IsNullOrEmpty(page.Icon))
                script.Add(new ChangePageIcon(page.Identifier, page.Icon));

            if (!string.IsNullOrEmpty(page.Hook))
                script.Add(new ChangePageHook(page.Identifier, page.Hook));

            if (!string.IsNullOrEmpty(page.ContentControl))
                script.Add(new ChangePageContentControl(page.Identifier, page.ContentControl));

            if (!string.IsNullOrEmpty(page.ContentLabels))
                script.Add(new ChangePageContentLabels(page.Identifier, page.ContentLabels));

            if (!string.IsNullOrEmpty(page.AuthorName))
                script.Add(new ChangePageAuthorName(page.Identifier, page.AuthorName));

            if (page.AuthorDate.HasValue)
                script.Add(new ChangePageAuthorDate(page.Identifier, page.AuthorDate));

            return script;
        }

        public List<ICommand> GetCommands(QPage page)
        {
            var script = new List<ICommand>
            {
                new CreatePage(page.PageIdentifier, page.SiteIdentifier, page.ParentPageIdentifier , page.OrganizationIdentifier, Guid.Empty, page.Title, page.PageType, page.Sequence, page.IsHidden, page.IsNewTab)
            };

            if (page.ParentPageIdentifier.HasValue)
                script.Add(new ChangePageParent(page.PageIdentifier, page.ParentPageIdentifier));

            if (!string.IsNullOrEmpty(page.PageSlug))
                script.Add(new ChangePageSlug(page.PageIdentifier, page.PageSlug));

            if (!string.IsNullOrEmpty(page.NavigateUrl))
                script.Add(new ChangePageNavigationUrl(page.PageIdentifier, page.NavigateUrl));

            if (!string.IsNullOrEmpty(page.PageIcon))
                script.Add(new ChangePageIcon(page.PageIdentifier, page.PageIcon));

            if (!string.IsNullOrEmpty(page.Hook))
                script.Add(new ChangePageHook(page.PageIdentifier, page.Hook));

            if (!string.IsNullOrEmpty(page.ContentControl))
                script.Add(new ChangePageContentControl(page.PageIdentifier, page.ContentControl));

            if (!string.IsNullOrEmpty(page.ContentLabels))
                script.Add(new ChangePageContentLabels(page.PageIdentifier, page.ContentLabels));

            if (!string.IsNullOrEmpty(page.AuthorName))
                script.Add(new ChangePageAuthorName(page.PageIdentifier, page.AuthorName));

            if (page.AuthorDate.HasValue)
                script.Add(new ChangePageAuthorDate(page.PageIdentifier, page.AuthorDate));


            return script;
        }

        public ICommand[] GetPageBlockCommands(QPage page)
        {
            var script = new List<ICommand>
            {
                new ChangePageTitle(page.PageIdentifier, page.Title),
                new ChangePageHook(page.PageIdentifier, page.Hook),
                new ChangePageSlug(page.PageIdentifier, page.PageSlug),
                new ChangePageContentControl(page.PageIdentifier, page.ContentControl)
            };

            return script.ToArray();
        }

        public ICommand[] GetDifferencePageSetupCommands(PageState original, PageState changed)
        {
            var script = new List<ICommand>();

            if (original.Title != changed.Title)
                script.Add(new ChangePageTitle(original.Identifier, changed.Title));

            if (original.Type != changed.Type)
                script.Add(new ChangePageType(original.Identifier, changed.Type));

            if (original.Slug != changed.Slug)
                script.Add(new ChangePageSlug(original.Identifier, changed.Slug));

            if (original.NavigateUrl != changed.NavigateUrl)
                script.Add(new ChangePageNavigationUrl(original.Identifier, changed.NavigateUrl));

            if (original.IsNewTab != changed.IsNewTab)
                script.Add(new ChangePageNewTabValue(original.Identifier, changed.IsNewTab));

            if (original.ParentPage != changed.ParentPage)
                script.Add(new ChangePageParent(original.Identifier, changed.ParentPage));

            if (original.Site != changed.Site)
                script.Add(new ChangePageSite(original.Identifier, changed.Site));

            if (original.Icon != changed.Icon)
                script.Add(new ChangePageIcon(original.Identifier, changed.Icon));

            if (original.Hook != changed.Hook)
                script.Add(new ChangePageHook(original.Identifier, changed.Hook));

            if (original.ContentControl != changed.ContentControl)
                script.Add(new ChangePageContentControl(original.Identifier, changed.ContentControl));

            // If the page has no content control then ensure the page is not assigned to any assessment, course,
            // program, or survey.

            if (changed.ContentControl == null)
            {
                if (original.Assessment.HasValue)
                    script.Add(new ChangePageAssessment(original.Identifier, null));

                if (original.Catalog.HasValue)
                    script.Add(new ModifyPageObject(original.Identifier, "Catalog", null));

                if (original.Course.HasValue)
                    script.Add(new ChangePageCourse(original.Identifier, null));

                if (original.Program.HasValue)
                    script.Add(new ChangePageProgram(original.Identifier, null));

                if (original.Survey.HasValue)
                    script.Add(new ChangePageSurvey(original.Identifier, null));
            }

            if (original.ContentLabels != changed.ContentLabels)
                script.Add(new ChangePageContentLabels(original.Identifier, changed.ContentLabels));

            if (original.AuthorName != changed.AuthorName)
                script.Add(new ChangePageAuthorName(original.Identifier, changed.AuthorName));

            if (original.AuthorDate != changed.AuthorDate)
                script.Add(new ChangePageAuthorDate(original.Identifier, changed.AuthorDate));

            if (original.Catalog != changed.Catalog)
                script.Add(new ModifyPageObject(original.Identifier, "Catalog", changed.Catalog));

            if (original.Course != changed.Course)
                script.Add(new ChangePageCourse(original.Identifier, changed.Course));

            if (original.Assessment != changed.Assessment)
                script.Add(new ChangePageAssessment(original.Identifier, changed.Assessment));

            if (original.Program != changed.Program)
                script.Add(new ChangePageProgram(original.Identifier, changed.Program));

            if (original.Survey != changed.Survey)
                script.Add(new ChangePageSurvey(original.Identifier, changed.Survey));

            return script.ToArray();
        }

        public List<ICommand> GetDifferencePageSetupCommands(QPage original, QPage changed)
        {
            var script = new List<ICommand>();

            if (IsStringChanged(original.Title, changed.Title))
                script.Add(new ChangePageTitle(original.PageIdentifier, changed.Title));

            if (IsStringChanged(original.PageType, changed.PageType))
                script.Add(new ChangePageType(original.PageIdentifier, changed.PageType));

            if (IsStringChanged(original.PageSlug, changed.PageSlug))
                script.Add(new ChangePageSlug(original.PageIdentifier, changed.PageSlug));

            if (IsStringChanged(original.NavigateUrl, changed.NavigateUrl))
                script.Add(new ChangePageNavigationUrl(original.PageIdentifier, changed.NavigateUrl));

            if (original.IsNewTab != changed.IsNewTab)
                script.Add(new ChangePageNewTabValue(original.PageIdentifier, changed.IsNewTab));

            if (original.ParentPageIdentifier != changed.ParentPageIdentifier)
                script.Add(new ChangePageParent(original.PageIdentifier, changed.ParentPageIdentifier));

            if (original.Site != changed.Site)
                script.Add(new ChangePageSite(original.PageIdentifier, changed.SiteIdentifier));

            if (IsStringChanged(original.PageIcon, changed.PageIcon))
                script.Add(new ChangePageIcon(original.PageIdentifier, changed.PageIcon));

            if (IsStringChanged(original.Hook, changed.Hook))
                script.Add(new ChangePageHook(original.PageIdentifier, changed.Hook));

            if (IsStringChanged(original.ContentControl, changed.ContentControl))
                script.Add(new ChangePageContentControl(original.PageIdentifier, changed.ContentControl));

            if (IsStringChanged(original.ContentLabels, changed.ContentLabels))
                script.Add(new ChangePageContentLabels(original.PageIdentifier, changed.ContentLabels));

            if (IsStringChanged(original.AuthorName, changed.AuthorName))
                script.Add(new ChangePageAuthorName(original.PageIdentifier, changed.AuthorName));

            if (original.AuthorDate != changed.AuthorDate)
                script.Add(new ChangePageAuthorDate(original.PageIdentifier, changed.AuthorDate));

            if (changed.ObjectType == "Assessment" && original.ObjectIdentifier != changed.ObjectIdentifier)
                script.Add(new ChangePageAssessment(original.PageIdentifier, changed.ObjectIdentifier));

            if (changed.ObjectType == "Catalog" && original.ObjectIdentifier != changed.ObjectIdentifier)
                script.Add(new ModifyPageObject(original.PageIdentifier, "Catalog", changed.ObjectIdentifier));

            if (changed.ObjectType == "Course" && original.ObjectIdentifier != changed.ObjectIdentifier)
                script.Add(new ChangePageCourse(original.PageIdentifier, changed.ObjectIdentifier));

            if (changed.ObjectType == "Program" && original.ObjectIdentifier != changed.ObjectIdentifier)
                script.Add(new ChangePageProgram(original.PageIdentifier, changed.ObjectIdentifier));

            if (changed.ObjectType == "Survey" && original.ObjectIdentifier != changed.ObjectIdentifier)
                script.Add(new ChangePageSurvey(original.PageIdentifier, changed.ObjectIdentifier));

            if (original.IsHidden != changed.IsHidden)
                script.Add(new ChangePageVisibility(original.PageIdentifier, changed.IsHidden));

            return script;

            bool IsStringChanged(string value1, string value2) => value1.NullIfEmpty() != value2.NullIfEmpty();
        }

        public ICommand[] DeletePageWithChildren(List<Guid> pagesToDele)
        {
            var script = new List<ICommand>();

            foreach (var pageId in pagesToDele)
            {
                script.Add(new DeletePage(pageId));
            }

            return script.ToArray();
        }
    }
}
