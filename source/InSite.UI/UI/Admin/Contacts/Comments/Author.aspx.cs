using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.People.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Comments.Forms
{
    public partial class Author : AdminBasePage, IHasParentLinkParameters
    {
        private Guid ContactIdentifier => Guid.TryParse(Request["contact"], out var value) ? value : Guid.Empty;

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

            var organizationId = GetOrganizationId();

            var person = ServiceLocator.PersonSearch.GetPerson(ContactIdentifier, organizationId);
            if (person == null)
                RedirectToParent();

            ServiceLocator.SendCommand(new ModifyPersonComment(person.PersonIdentifier, CommentActionType.Author, comment));

            if (comment != null)
                RedirectToParent();
        }

        /// <remarks>
        /// If the person is assigned to the current organization then assume the current organization owns the comment.
        /// Otherwise, get the list of organizations assigned to the person, sort alphabetically by organization code, 
        /// and assume the first organization in the list owns the comment.
        /// </remarks>
        private Guid GetOrganizationId()
        {
            var organizationId = Organization.Identifier;

            var people = ServiceLocator.PersonSearch.GetPersons(new QPersonFilter { UserIdentifier = ContactIdentifier }, x => x.Organization);

            if (people.Count > 0 && !people.Any(p => p.OrganizationIdentifier == organizationId))
            {
                organizationId = people.OrderBy(p => p.Organization.OrganizationCode).First().OrganizationIdentifier;
            }

            return organizationId;
        }

        private void SendEmailNotification(QComment comment)
        {
            var author = UserSearch.Bind(comment.AuthorUserIdentifier, x => new { x.FirstName, x.LastName, x.Email });
            var topic = UserSearch.Bind(comment.TopicUserIdentifier.Value, x => new { x.FirstName, x.LastName, x.Email });

            ServiceLocator.AlertMailer.Send(Organization.Identifier, User.Identifier,
                new Notification_PersonCommentFlagged
                {
                    AuthorFirstName = author.FirstName,
                    AuthorLastName = author.LastName,
                    AuthorEmail = author.Email,
                    TopicFirstName = topic.FirstName,
                    TopicLastName = topic.LastName,
                    TopicEmail = topic.Email,
                    CommentText = comment.CommentText
                });
        }

        private void Open()
        {
            var user = UserSearch.Bind(ContactIdentifier, x => new { x.UserIdentifier, x.FullName });

            if (user == null)
                RedirectToSearch();

            SetInputValues(user.UserIdentifier, user.FullName);
        }

        private PersonComment Get()
        {
            if (!Page.IsValid)
                return null;

            if (string.IsNullOrWhiteSpace(CommentText.Text))
            {
                EditorStatus.AddMessage(AlertType.Error, "Text of comment can not be empty");
                return null;
            }

            return new PersonComment
            {
                Comment = UniqueIdentifier.Create(),
                Text = CommentText.Text,
                Flag = CommentFlag.FlagValue?.GetName(),
                IsPrivate = CommentIsPrivate.Checked,
                Resolved = CommentResolved.Value,

                Container = ContactIdentifier,
                ContainerType = "Person",

                Author = User.UserIdentifier,
                AuthorName = User.FullName,

                Organization = Organization.Identifier,

                Topic = ContactIdentifier
            };
        }

        private void SetInputValues(Guid userId, string userName)
        {
            PageHelper.AutoBindHeader(this, null, userName);

            CommentText.LoadData(userId);
            CommentIsPrivate.Checked = true;
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