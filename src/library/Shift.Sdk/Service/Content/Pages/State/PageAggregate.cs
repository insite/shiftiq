using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Sites.Pages
{
    public class PageAggregate : AggregateRoot
    {
        #region Properties (state)

        public override AggregateState CreateState() => new PageState();
        public PageState Data => (PageState)State;

        public void CreatePage(Guid? site, Guid? parentPage, Guid organization, Guid author, string title, string type, int sequence, bool isHidden, bool isNewTab)
        {
            Apply(new PageCreated(site, parentPage, organization, author, title, type, sequence, isHidden, isNewTab));
        }

        public void ChangePageContent(ContentContainer content)
        {
            if (!ContentContainer.IsEqual(Data.Content, content))
                Apply(new PageContentChanged(content));
        }

        public void ChangePageAuthorName(string authorName)
        {
            Apply(new AuthorNameChanged(authorName));
        }
        public void ChangePageSequence(int sequence)
        {
            Apply(new SequenceChanged(sequence));
        }
        public void ChangePageNewTabValue(bool isNewTab)
        {
            Apply(new NewTabValueChanged(isNewTab));
        }
        public void ChangePageVisibility(bool isHidden)
        {
            Apply(new VisibilityChanged(isHidden));
        }
        public void ChangePageTitle(string title)
        {
            Apply(new TitleChanged(title));
        }
        public void ChangePageSlug(string slug)
        {
            Apply(new SlugChanged(slug));
        }
        public void ChangePageIcon(string icon)
        {
            Apply(new IconChanged(icon));
        }
        public void ChangePageNavigationUrl(string navigateUrl)
        {
            Apply(new NavigationUrlChanged(navigateUrl));
        }
        public void ChangePageContentLabels(string contentLabels)
        {
            Apply(new ContentLabelsChanged(contentLabels));
        }
        public void ChangePageHook(string hook)
        {
            Apply(new HookChanged(hook));
        }
        public void ChangePageContentControl(string contentControl)
        {
            Apply(new ContentControlChanged(contentControl));
        }
        public void ChangePageType(string type)
        {
            Apply(new TypeChanged(type));
        }

        public void ChangePageParent(Guid? parent)
        {
            Apply(new ParentChanged(parent));
        }
        public void ChangePageSite(Guid? site)
        {
            Apply(new SiteChanged(site));
        }
        public void ChangePageCourse(Guid? course)
        {
            Apply(new CourseChanged(course));
        }
        public void ChangePageAssessment(Guid? assessment)
        {
            Apply(new PageAssessmentChanged(assessment));
        }
        public void ChangePageProgram(Guid? program)
        {
            Apply(new ProgramChanged(program));
        }

        public void ModifyPageObject(string type, Guid? @object)
        {
            Apply(new PageObjectModified(type, @object));
        }

        public void ChangePageSurvey(Guid? survey)
        {
            Apply(new SurveyChanged(survey));
        }
        public void ChangePageAuthorDate(DateTimeOffset? date)
        {
            Apply(new AuthorDateChanged(date));
        }

        public void DeletePage()
        {
            Apply(new PageDeleted());
        }

        #endregion
    }
}
