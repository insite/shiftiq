using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{

    public class PageState : AggregateState
    {
        /// <summary>
        /// Uniquely identifies the site itself.
        /// </summary>
        public Guid Identifier { get; set; }
        public Guid Tenant { get; set; }
        public Guid Author { get; set; }

        public Guid? Assessment { get; set; }
        public Guid? Catalog { get; set; }
        public Guid? Course { get; set; }
        public Guid? Program { get; set; }
        public Guid? Survey { get; set; }
        public Guid? ParentPage { get; set; }
        public Guid? Site { get; set; }

        public string AuthorName { get; set; }
        public string ContentControl { get; set; }
        public string ContentLabels { get; set; }
        public string Hook { get; set; }
        public string NavigateUrl { get; set; }
        public string Icon { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }

        public bool IsHidden { get; set; }
        public bool IsNewTab { get; set; }

        public int Sequence { get; set; }

        public DateTimeOffset? AuthorDate { get; set; }
        public Shift.Common.ContentContainer Content { get; set; }


        public PageState()
        {

        }

        public void When(AuthorNameChanged c)
        {
            AuthorName = c.AuthorName;
        }

        public void When(ContentControlChanged c)
        {
            ContentControl = c.ContentControl;
        }

        public void When(ContentLabelsChanged c)
        {
            ContentLabels = c.ContentLabels;
        }

        public void When(HookChanged c)
        {
            Hook = c.Hook;
        }

        public void When(IconChanged c)
        {
            Icon = c.Icon;
        }

        public void When(NavigationUrlChanged c)
        {
            NavigateUrl = c.NavigateUrl;
        }

        public void When(NewTabValueChanged c)
        {
            IsNewTab = c.IsNewTab;
        }

        public void When(SequenceChanged c)
        {
            Sequence = c.Sequence;
        }

        public void When(SlugChanged c)
        {
            Slug = c.Slug;
        }

        public void When(TitleChanged c)
        {
            Title = c.Title;
        }

        public void When(TypeChanged c)
        {
            Type = c.Type;
        }

        public void When(VisibilityChanged c)
        {
            IsHidden = c.IsHidden;
        }

        public void When(ParentChanged c)
        {
            ParentPage = c.Parent;
        }

        public void When(SiteChanged c)
        {
            Site = c.Site;
        }

        public void When(SurveyChanged c)
        {
            Survey = c.Survey;
        }

        public void When(CourseChanged c)
        {
            Course = c.Course;
        }

        public void When(PageAssessmentChanged c)
        {
            Assessment = c.Assessment;
        }

        public void When(PageObjectModified c)
        {
            Assessment = null;
            Course = null;
            Program = null;
            Survey = null;

            if (c.Object != null)
            {
                switch (c.Type)
                {
                    case "Assessment":
                        Assessment = c.Object;
                        break;

                    case "Course":
                        Course = c.Object;
                        break;

                    case "Program":
                        Program = c.Object;
                        break;

                    case "Survey":
                        Survey = c.Object;
                        break;
                }
            }
        }

        public void When(ProgramChanged c)
        {
            Program = c.Program;
        }

        public void When(AuthorDateChanged c)
        {
            AuthorDate = c.AuthorDate;
        }

        public void When(PageDeleted _)
        {
        }

        public void When(PageContentChanged e)
        {
            Content = e.Content;
        }

        public void When(PageCreated e)
        {
            Identifier = e.AggregateIdentifier;

            Tenant = e.Tenant;
            Author = e.Author;

            Type = e.Type;
            Title = e.Title;

            IsHidden = e.IsHidden;
            IsNewTab = e.IsNewTab;

            Sequence = e.Sequence;
        }
    }
}
