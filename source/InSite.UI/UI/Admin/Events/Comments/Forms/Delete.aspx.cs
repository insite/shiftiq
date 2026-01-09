using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Events.Comments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid EventID => Guid.TryParse(Request.QueryString["event"], out var value) ? value : Guid.Empty;

        private Guid CommentID => Guid.TryParse(Request.QueryString["id"], out Guid value) ? value : Guid.Empty;

        private EventType EventType => Request.UrlReferrer == null || Request.UrlReferrer.ToString().Contains("class")
            ? EventType.Class
            : EventType.Exam;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var @event = ServiceLocator.EventSearch.GetEvent(EventID);

            if (CommentID != Guid.Empty)
            {
                var comment = ServiceLocator.EventSearch.GetComment(CommentID);

                if (comment == null)
                    RedirectToOutline();
                var user = UserSearch.Bind(comment.AuthorUserIdentifier, x => x.FullName) ?? String.Empty;
                PostedBy.Text = !string.IsNullOrEmpty(user) ? $"<a href=\"/ui/admin/contacts/people/edit?contact={comment.AuthorUserIdentifier}\">{user}</a>" : UserNames.Someone;
                CommentText.Text = comment.CommentText.Replace("\n", "<br>");
                PostedOn.Text = GetLocalTime(comment.CommentPosted);
            }

            var titleSubtext = EventType == EventType.Exam ? String.Empty : "scheduled " + @event.EventScheduledStart.FormatDateOnly(User.TimeZone);

            if (EventType == EventType.Exam)
            {
                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Events", "/ui/admin/events/home"),
                    new BreadcrumbItem("Exams", "/ui/admin/events/exams/search"),
                    new BreadcrumbItem("Outline", $"/ui/admin/events/exams/outline?event={EventID}"),
                    new BreadcrumbItem("Delete Comment", null, null, "active"),
                }, null, $"{@event.EventTitle} <span class='form-text'>{titleSubtext}</span>");

                EventTitle.Text = $"<a href=\"/ui/admin/events/exams/outline?event={EventID}\">{@event.EventTitle}</a>";
            }
            else
            {
                PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Events", "/ui/admin/events/home"),
                    new BreadcrumbItem("Classes", "/ui/admin/events/classes/search"),
                    new BreadcrumbItem("Outline", $"/ui/admin/events/classes/outline?event={EventID}"),
                    new BreadcrumbItem("Delete Comment", null, null, "active"),
                }, null, $"{@event.EventTitle} <span class='form-text'>{titleSubtext}</span>");

                EventTitle.Text = $"<a href=\"/ui/admin/events/classes/outline?event={EventID}\">{@event.EventTitle}</a>";
            }

            WarningText.Text = $"This is a permanent change that cannot be undone. \n" +
                $"The comment will be deleted from the {(EventType == EventType.Exam ? "exam" : "class event")}. \n" +
                $"Here is a summary of the data that will be erased if you proceed.";
        }

        private void RedirectToOutline()
        {
            if (EventType == EventType.Exam)
                HttpResponseHelper.Redirect($"/ui/admin/events/exams/outline?event={EventID}&panel=comment");
            else
                HttpResponseHelper.Redirect($"/ui/admin/events/classes/outline?event={EventID}&panel=comment");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new RemoveComment(EventID, CommentID));

            RedirectToOutline();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RedirectToOutline();
        }

        private string GetLocalTime(DateTimeOffset? item)
        {
            return item.Format(User.TimeZone, nullValue: "None", isHtml: true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            if (EventType == EventType.Exam)
                return parent.Name.EndsWith("/events/exams/edit")
                ? $"event={EventID}"
                : null;
            else
                return parent.Name.EndsWith("/events/classes/outline")
                ? $"event={EventID}"
                : null;
        }
    }
}