using System;

using InSite.Application.Contents.Read;
using InSite.Application.People.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Comments.Forms
{
    public partial class Revise : AdminBasePage, IHasParentLinkParameters
    {
        private Guid ContactIdentifier => Guid.TryParse(Request["contact"], out var value) ? value : Guid.Empty;

        private Guid CommentKey => Guid.Parse(Request["comment"]);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var comment = Get();

            if (comment == null)
                return;

            var person = ServiceLocator.PersonSearch.GetPerson(ContactIdentifier, Organization.Identifier);
            if (person == null)
                RedirectToParent();

            ServiceLocator.SendCommand(new ModifyPersonComment(person.PersonIdentifier, CommentActionType.Revise, comment));

            if (comment != null)
                RedirectToParent();
        }

        private void Open()
        {
            var user = UserSearch.Bind(ContactIdentifier, x => new { x.UserIdentifier, x.FullName });
            if (user == null)
                RedirectToSearch();

            var comment = QCommentSearch.SelectFirst(CommentKey);
            if (comment == null)
                RedirectToParent();

            PageHelper.AutoBindHeader(this, null, user.FullName);

            SetInputValues(user.UserIdentifier, comment);
        }

        private PersonComment Get()
        {
            if (!Page.IsValid)
                return null;

            if (CommentText.Text == string.Empty)
            {
                EditorStatus.AddMessage(AlertType.Error, "Text of comment can not be empty");
                return null;
            }

            var comment = QCommentSearch.SelectFirst(CommentKey);

            if (comment == null)
                return null;

            return new PersonComment
            {
                Comment = CommentKey,
                Text = CommentText.Text,
                Flag = CommentFlag.FlagValue?.GetName(),
                IsPrivate = CommentIsPrivate.Checked,
                Resolved = CommentResolved.Value,
                Revised = DateTimeOffset.Now,

                Container = comment.ContainerIdentifier,
                ContainerType = "Person",

                Author = comment.AuthorUserIdentifier,
                AuthorName = comment.AuthorUserName,

                Organization = Organization.Identifier,

                Topic = comment.TopicUserIdentifier
            };
        }

        private void SetInputValues(Guid userId, QComment comment)
        {
            CommentText.LoadData(userId, comment);
            CommentFlag.FlagValue = Enum.TryParse<FlagType>(comment.CommentFlag, out var flag) ? flag : (FlagType?)null;
            CommentIsPrivate.Checked = comment.CommentIsPrivate;
            CommentResolved.Value = comment.CommentResolved;

            RemoveButton.NavigateUrl = $"/ui/admin/contacts/comments/delete?contact={ContactIdentifier}&comment={CommentKey}";
            CancelButton.NavigateUrl = GetParentUrl();
        }

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/contacts/people/search", true);

        private void RedirectToParent()
        {
            var url = GetParentUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetParentUrl()
        {
            var url = new ReturnUrl().GetReturnUrl();

            if (url == null)
                url = $"/ui/admin/contacts/people/edit?contact={ContactIdentifier}&panel=comments";

            return url;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={ContactIdentifier}"
                : null;
        }
    }
}
