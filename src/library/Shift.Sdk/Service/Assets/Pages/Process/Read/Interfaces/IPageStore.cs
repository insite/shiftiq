using System;

using InSite.Domain.Sites.Pages;

namespace InSite.Application.Sites.Read
{
    public interface IPageStore
    {
        void InsertPage(PageCreated change);

        void UpdatePage(AuthorNameChanged page);

        void UpdatePage(ContentControlChanged page);

        void UpdatePage(ContentLabelsChanged page);

        void UpdatePage(HookChanged page);

        void UpdatePage(IconChanged page);

        void UpdatePage(NavigationUrlChanged page);

        void UpdatePage(NewTabValueChanged page);

        void UpdatePage(SequenceChanged page);

        void UpdatePage(SlugChanged page);

        void UpdatePage(TitleChanged page);

        void UpdatePage(TypeChanged page);

        void UpdatePage(VisibilityChanged page);

        void UpdatePage(PageContentChanged site);

        void UpdatePage(ParentChanged page);
        void UpdatePage(SiteChanged page);
        void UpdatePage(SurveyChanged page);
        void UpdatePage(CourseChanged page);
        void UpdatePage(PageAssessmentChanged page);
        void UpdatePage(PageObjectModified page);
        void UpdatePage(ProgramChanged page);
        void UpdatePage(AuthorDateChanged date);

        void DeleteAll();
        void DeleteOne(Guid page);
        void DeletePage(PageDeleted change);
    }
}
