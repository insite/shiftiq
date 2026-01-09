using System;

using Shift.Common.Timeline.Commands;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Comments.Forms
{
    public partial class Post : AdminBasePage, IHasParentLinkParameters
    {
        private Guid EventIdentifier => Guid.TryParse(Request.QueryString["event"], out var value) ? value : Guid.Empty;

        private Guid? CommentIdentifier => Guid.TryParse(Request.QueryString["id"], out Guid value) ? value : (Guid?)null;

        private EventType EventType => Request.UrlReferrer == null || Request.UrlReferrer.ToString().Contains("class") 
            ? EventType.Class
            : EventType.Exam;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SaveButton.Click += OnConfirmed;
            CancelButton.Click += OnCancelled;
        }

        private void OnConfirmed(object sender, EventArgs e)
        {
            if (Page.IsValid)
                Save();
        }

        private void OnCancelled(object sender, EventArgs e)
        {
            RedirectToOutline();
        }

        private void Save()
        {
            var post = CommentIdentifier == null
                ? (Command)new PostComment(EventIdentifier, UniqueIdentifier.Create(), User.UserIdentifier, CommentText.Text)
                : (Command)new ReviseComment(EventIdentifier, CommentIdentifier.Value, User.UserIdentifier, CommentText.Text);
            ServiceLocator.SendCommand(post);
            RedirectToOutline();
        }

        private void RedirectToOutline()
        {
            if (EventType == EventType.Exam)
                HttpResponseHelper.Redirect($"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=comment");
            else
                HttpResponseHelper.Redirect($"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=comment");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Identity.IsGranted(Route.ToolkitName, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/events/classes/search");

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier);

            if (@event == null)
                HttpResponseHelper.Redirect("/ui/admin/events/classes/search");

            if (EventType == EventType.Exam)
                PageHelper.AutoBindHeader(this, null, @event.EventTitle);
            else
            {
                var title = $"{@event.EventTitle} <span class='form-text'>scheduled {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>";
                PageHelper.AutoBindHeader(this, null, title);
            }

            CommentPanel.LoadData(EventIdentifier, EventType);

            if (CommentIdentifier != null)
            {
                var comment = ServiceLocator.EventSearch.GetComment(CommentIdentifier.Value);

                if (comment == null)
                    RedirectToOutline();

                CommentText.Text = comment.CommentText;
            }
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            if (EventType == EventType.Exam)
                return parent.Name.EndsWith("/events/exams/edit")
                ? $"event={EventIdentifier}"
                : null;
            else
                return parent.Name.EndsWith("/events/classes/outline")
                ? $"event={EventIdentifier}"
                : null;
        }
    }
}
